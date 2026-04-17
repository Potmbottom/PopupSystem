# Popup Showcase Architecture

This project is a Unity showcase built around one core concern: a popup system that can accept requests from gameplay code, resolve whether the popup comes from a local prefab or a remote bundle, order requests by priority, and keep the screen in a valid state while popups open and close. Most of the code lives in `PopupSystem/Assets/_Project/Scripts`, split into small assemblies so the popup domain, game-specific meta flow, asset loading, and shared UI helpers stay separated.

## Popup System

The popup system is the actual runtime engine of the project. It is intentionally split into three layers.

`PopupData` is the contract layer. `BasePopupData` defines the common popup model: popup type, priority, activation signal, close signal, and disposable lifetime. Concrete popup payloads such as login, loading, offer, or tutorial-complete inherit from it and carry only the data and commands needed by that popup.

`PopupSystem` is the orchestration layer. `PopupRequestService` is the API that feature code uses to request a popup. It looks up `PopupPrefabConfig` and decides whether the popup should be shown from a local prefab immediately or whether it must first load a prefab through `IRemoteContentProvider`. It also deduplicates concurrent remote loads per popup type, so the same remote popup is not requested twice while one load is already in progress.

The queue itself is priority-first, FIFO-second. `PopupQueueProvider` owns one `PopupQueue` per `PopupPriority`. `PopupQueueAggregate` watches those queues and always exposes the currently active item from the highest-priority non-empty queue. In practice this means a system interrupt or loading popup can temporarily take the screen away from a standard popup, and when the higher-priority popup closes, the previous popup becomes current again without rebuilding the queue state.

Rendering is handled by `PopupContainer`. It subscribes to the current queue item, releases the old popup, creates or reuses the next popup control, binds the popup data model to it, and toggles the modal blocker. Local prefabs are pooled by popup type. Remote prefabs are instantiated for the request and destroyed after use, while their underlying loaded assets are released through the handler-based remote content lifetime.

For popups, `BasePopupData` and its subclasses are the popup-side models. Their responsibility is to carry popup state, expose signals such as activation, close, login request, or buy request, and define popup identity and priority. `BasePopupControl<T>` is the popup-side control. It bridges a `BasePopupData` model into a MonoBehaviour, runs the open transition, and closes through the model so the queue can react through the `Closed` observable rather than through direct references. Concrete popup controls mostly bind buttons, text, and images. `PopupTransition` and `PopupBlocker` keep animation and modality outside of queue logic.

The runtime flow is:

1. Feature code creates a `BasePopupData` subclass and sends it to `IPopupRequestService` or directly to `PopupQueueProvider`.
2. The request service resolves prefab source from `PopupPrefabConfig`, loading a remote prefab if required.
3. `PopupQueueProvider` inserts the request into the queue that matches its priority.
4. `PopupQueueAggregate` selects the highest-priority visible popup and publishes it as the current item.
5. `PopupContainer` creates or reuses the visual control, binds the data, and shows the blocker.
6. The popup control calls `Model.Close()`, which makes the provider dequeue and dispose the popup, release remote handles if needed, and reveal the next eligible popup.

This design keeps queue rules independent from visuals. The queue has no knowledge of Unity hierarchy, transitions, buttons, or sprite loading. The controls have no knowledge of queue ordering. The request service is the only place where prefab source selection and remote loading policy are decided.

## Meta Part

The UI side follows a Model -> Control pattern, both for the meta screens and for popups. Models own state, expose reactive streams, and contain the business rules that decide what should happen. Controls are MonoBehaviours that bind to that state, forward user input into model commands, and update visuals. They should react to state, not invent gameplay rules locally.

The codebase is also divided by `.asmdef` boundaries, so popup data, popup runtime, meta flow, assets, configs, core helpers, infrastructure, editor tooling, and tests stay as separate compile units instead of one large Unity assembly.

The `Meta` assembly is a thin game layer that demonstrates how a real feature set drives the popup engine. `GamePresenter` runs the startup flow, loads player state, opens login and daily reward popups, reacts to menu actions, and triggers tutorial, offer, loading, and system-interrupt popups. `PlayerStateModel`, `OffersModel`, `MenuModel`, and popup data objects hold the game-facing reactive state and decisions, while `MenuControl`, `OffersShowControl`, `OfferCardControl`, and the popup-specific controls only render that state and forward user actions back into the models.

The important boundary is that meta code only speaks in terms of models and popup requests. It does not manually instantiate popup prefabs or manage popup lifetime itself. Business logic stays in presenter and model layers. Controls stay passive and view-focused. That keeps the showcase logic replaceable without changing the popup engine.

## Everything Else

`Assets` contains the loading backends. Addressables are used for local addressable assets, asset bundles are used for remote popup prefabs and remote sprites, and `SpriteLoader` hides the difference so UI code can request a sprite path without caring where it comes from.

`Configs` contains the ScriptableObject configuration that wires popup types to prefab sources, defines offer catalog data, and holds game-level settings such as loading delays and fallback sprite paths. `Core` contains reusable UI primitives like `BaseControl`, `BasePresenter`, tween helpers, and the `ExtendedImage` component that manages async sprite lifetime safely. `Editor` contains convenience tooling for rebuilding remote bundles and resetting mock player state. `Tests/EditMode` is the test directory and covers the highest-risk popup behavior: queue priority restoration, duplicate enqueue rejection, remote-popup deduplication, and cleanup on remote load failure.

## Tech Stack

The project uses Zenject for dependency injection, UniTask for async flows, R3 for reactive state and event streams, and DOTween for UI transitions and motion.

The application uses AssetBundles, Addressables, and atlases. Addressables are the local asset source, AssetBundles are the remote delivery path, and atlases are part of the UI asset packaging strategy.

## Showcase

The showcase consists of 6 popups.

1. `SystemPopup`. Error handling popup with top priority.
2. `LoadingBundlesPopup`. Loading popup with top priority.
3. `DailyPopup`. Standard priority popup.
4. `TutorFinishPopup`. Standard priority popup loaded with bundles.
5. `LoginPopup`. Standard priority popup.
6. `GenericOfferPopup`. Configurable from config and shown with low priority.

Expected game flow:

1. Start the game and wait through the loading screen.
2. Show `LoginPopup`.
3. Show `DailyPopup`.
4. Buy a local offer if desired.
5. Press the complete tutorial button.
6. Wait for bundle download.
7. Show `TutorFinishPopup`.
8. Buy a remote-loaded offer.

Optional debug flow:

1. The red debug button creates a system popup.
2. The green debug button creates a default popup.

Tools\Debug\Reset TO default player state
