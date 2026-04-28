# Unity Technical Assessment: Advanced Popup System

## Overview
Your objective is to design and implement a robust, production-grade Popup System for Unity. The system must be capable of managing multiple UI elements with varying priorities, sources, and lifecycles. We are looking for a solution that demonstrates deep architectural understanding, clean C# practices, and optimal Unity-specific performance.

## Core Requirements

### 1. Management & Sequencing
- **Queue Logic:** Implement a system to manage multiple popup requests. Support priority-based sequencing where certain popups can interrupt or wait for others.
- **Lifecycle Control:** Define a strict and predictable lifecycle for every popup (Initialization, Opening, Active, Closing, and Disposal).

### 2. Sourcing & Data
- **Local Popups:** Support for UI elements bundled within the primary application package.
- **Remote Popups:** Support for popups where either the asset itself, the content (images/text), or the configuration is retrieved from an external source.
- **Data Injection:** A clean, type-safe mechanism to pass data into a popup before it is displayed.

### 3. Visuals & Interaction
- **Transition System:** A flexible way to handle how popups appear and disappear.
- **Input & Modality:** Proper handling of user interaction, ensuring that modal popups correctly block background inputs and manage the "backdrop" behavior.

## Technical Expectations

### 1. Architectural Integrity
- Demonstrate a clear separation of concerns. The logic governing the queue should be independent of the UI rendering.
- The system should be highly extensible, allowing another developer to add new popup types or behaviors without modifying the core engine.

### 2. Asynchronous Operations
- Handle all loading and network-related operations gracefully. The UI must remain responsive during these processes.

### 3. Memory & Performance
- Efficient asset management: Ensure assets are loaded and unloaded correctly to prevent memory leaks.
- Object Lifecycle: Implement strategies to minimize runtime overhead and garbage collection (e.g., reuse mechanisms).
- UI Performance: Ensure the system does not cause unnecessary performance hits (e.g., layout overhead or draw call spikes).

### 4. Robustness
- **Error Handling:** Gracefully handle edge cases such as failed remote retrievals, missing assets, or rapid-fire user input.
- **State Management:** The system must remain in a valid state regardless of how many popups are queued, opened, or closed simultaneously.

## Evaluation Criteria
- **Quality of Code:** Cleanliness, naming conventions, and adherence to professional C# standards.
- **Unity Best Practices:** Proper use of Unity-specific primitives and optimization techniques.
- **Developer Experience:** How intuitive is the API for a developer using your system to trigger a new popup?
- **Testing & Validation:** Evidence that the core logic (especially the queue and priority system) is verified and reliable.

## Submission Guidelines
1. Provide a Unity project or a link to a Git repository.
2. Include a `README.md` that provides a high-level overview of your architecture and the reasoning behind your technical choices.
3. The project should include a demonstration scene that showcases the system's capabilities (e.g., a mix of priorities and sources).
