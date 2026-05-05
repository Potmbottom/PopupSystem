# Requirements
root\Requirements.md

# Assets
https://assetstore.unity.com/packages/2d/gui/hyper-casual-mobile-gui-268659

# Popup Showcase

Unity showcase of a popup system that takes requests from feature code, resolves the prefab source (local or Addressable), orders by priority, and keeps the screen consistent as popups open and close. Code lives in `Assets/Scripts`, split across `.asmdef` boundaries (popup runtime, MVVM flow, assets, scriptables, common helpers, tests).

## Architecture

Three layers, separated by intent:

- **Models** — `BasePopupModel` and subclasses carry popup state and signals (activated, closed, plus per-popup commands like `LoginRequested`, `BuyRequested`). No Unity references.
- **Queue** — `PopupQueueProvider` owns one `PopupQueue` per `PopupPriority`. `PopupQueueAggregate` exposes the highest-priority non-empty queue's head as the current item, so a system interrupt can preempt a standard popup and the previous popup resurfaces when the interrupt closes.
- **Presentation** — `PopupPresenter` subscribes to the current item, instantiates via `PopupFactory`, pools instances by type, and drives `PopupBlockerPresenter` for modality. `PopupTransitionPresenter` handles open/close animation.

`PopupRequestService` is the entry point feature code uses. It looks up `PopupPrefabConfig`, picks the local prefab or loads the Addressable, caches the loaded handle for reuse, and dedupes concurrent loads of the same type. Local-only popups can also enqueue directly through `PopupQueueProvider` (used by `GamePresenter` for synchronous flows).

The queue knows nothing about Unity. The views know nothing about queue ordering. Asset loading lives behind `IAssetProvider`.

## Tech Stack

Zenject (DI), UniTask (async), R3 (reactive), DOTween (motion), Addressables (asset loading).

## How to add a popup

1. Add a value to `PopupType`.
2. Subclass `BasePopupModel` with the popup's state, signals, and `PopupPriority`.
3. Subclass `BasePopupView<TModel>`, bind UI in `OnPopupModelUpdate`, call `ClosePopup()` to close.
4. Create the prefab with the view on its root.
5. Add an entry to `PopupPrefabConfig` with **exactly one** of `Prefab` (local) or `Address` (Addressable).
6. Enqueue via `IPopupRequestService.EnqueueAsync(model, ct)`.

## Showcase popups

| Popup | Priority | Source |
|---|---|---|
| `SystemInterrupt` | SystemInterrupt | local |
| `Loading` | SystemInterrupt | local |
| `Login` | Standard | local |
| `DailyReward` | Standard | local |
| `TutorialComplete` | Standard | Addressable |
| `Offer` | Standard | local (sprite via Addressable) |

Expected flow: startup loading → `Login` → `DailyReward` → optional offer → tutorial button triggers Addressable load → `TutorialComplete` → remote-sprite offer. Red/green debug buttons enqueue system-error and daily-reward popups. `Tools > Debug > Reset to default player state` resets PlayerPrefs.

## Tests

`Assets/Tests/EditMode/PopupQueueProviderTests.cs` covers priority restoration and duplicate-enqueue rejection.
