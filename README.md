# MMLibrary
A collection of Unity utilities and systems shared across multiple projects.

---

## Overview
Includes new collection types, managed update loops, `ScriptableObject`-based data types, singleton management, common runtime and editor utilities.
Systems providing runtime orchestration provide overrideable configurations for different projects.

---

## Table of Contents

- [Collections](#collections)
- [Controlled Updates](#controlled-updates)
- [Scriptable Object Framework](#scriptable-object-framework)
- [Singletons](#singletons)
- [Time Controls](#time-controls)
- [Utilities](#utilities)
- [Installation](#installation)
- [License](#license)

---

## Collections

### IndexedHashSet
A hybrid data structure combining the fast lookup of a `HashSet` with the ordered indexing of a `List`:
- Each item can only be represented once
- Add and Remove are both O(1) (ish, like a hash set)
- Indexed iteration / enumeration is fast (dense array)
- Order is not guaranteed after Remove is first called (quick swap remove)

---

## Controlled Updates

Provides a dependable, extensible update-ordering system for `MonoBehaviour` components.  
Useful for systems where deterministic update order is required, and provides a hard-coded diffable source of truth for this update order.

**Core classes:**
- `MonoBehaviourController` - Persistent singleton responsible for triggering updates
- `MonoBehaviourControlled` - `MonoBehaviour` that automatically registers for controlled updated based on `IControlled` implementations.
- `IControlled` - Optionally, any `object` can register for `MonoBehaviourController` update actions by implementing `IControlled`, although the register and deregister calls need to be handled manually.
- `ControlledUpdateOrderProvider` - Overridable list to define custom project-wide update order

**Project setup:**
1. Override `OverrideInitialisationOrder` to specify the order of singleton types in `ControlledUpdateOrder` (Note: All included types here should implement `IControlled`)
2. Optionally: Add `MonoBehaviourControlled` instance to a scene (ideally the first loaded scene of a project at least, but it can exist in any scene for testing). This is optional because the instance will otherwise be lazily initialised by any `MonoBehaviourControlled` initialisations.

**Usage (MonoBehaviour):**
1. Override `MonoBehaviourControlled` instead of `MonoBehaviour` for new `MonoBehaviour` classes.
2. Implement any combination of `IControlledUpdate`, `IControlledLateUpdate`, `IControlledFixedUpdate` and `IControlledDebugUpdate` and add their callback implementations.
3. Add the newly created `MonoBehaviourControlled` subclass to the project's `OverrideInitialisationOrder.ControlledUpdateOrder` override.

**Usage (custom object):**
1. Implement `IControlled` and any combination of `IControlledUpdate`, `IControlledLateUpdate`, `IControlledFixedUpdate` and `IControlledDebugUpdate` and add their callback implementations.
2. On initialisation, register for controlled updates:
```csharp
MM.MonoBehaviourController controller = MM.MonoBehaviourController.GetOrMakeInstance;
if( controller )
{
	controller.Register( this );
}
```
3. On destroy or dispose, unregister:
```csharp
MM.MonoBehaviourController controller = MM.MonoBehaviourController.GetOrMakeInstance;
if( controller )
{
	controller.Unregister( this );
}
```

---

## Scriptable Object Framework

A set of serialisable, event-driven systems to manage data and logic via `ScriptableObject` assets, enabling decoupled design and runtime flexibility.

**Core components:**
- `SEvent` — Scriptable event channels for decoupled communication.
- `SValue` — Serialised value containers for shared runtime data. Optionally these can revert to their default values on exiting play mode to prevent serialisation of runtime modifications.
- `SObservable` — Observable `SValue` types with optional listener callbacks (both pre-change and post-change).

The following basic types have been implemented already:

| Type | Event | Value | Observable |
|---|---|---|---|
| **Primitive** | | | |
| bool | ✔ | ✔ | ✔ |
| float | ✔ | ✔ | ✔ |
| int | ✔ | ✔ | ✔ |
| string | ✔ | ✔ | ✔ |
| **Composite** | | | |
| color | ✔ | ✔ | ✔ |
| vector3 | ✔ | ✔ | ✔ |
| vector2 | ✔ | ✔ | ✔ |
| quaternion | ✔ | ✔ | ✔ |
| **Asset** | | | |
| object | ✔ | ✔ | |
| material | ✔ | ✔ | |
| mesh | ✔ | ✔ | |
| texture | ✔ | ✔ | |
| **Scene** | | | |
| gameobject | ✔ | | |
| meshrenderer | ✔ | | |
| collider | ✔ | | |
| transform | ✔ | | |

Any new project-specific types can be easily created in the same manner.

---

## Singletons

Provides lightweight and fully serialisable singleton patterns, with editor integration and explicit initialization order control.

While it's possible to manage a project with multiple individual singletons (e.g. using `StandaloneSingleton`s), it quickly becomes difficult to manage their scene initialisations due to race conditions, dependencies, etc. This `SingletonHub` framework provides a central location in a scene to:
- Assign singletons and toggle on/off project singletons
- Guarantee the initialisation orders
- Control their scene-specific config data e.g. for testing scenes
- Manage global runtime access
- Manage optional lazy updates and lazy initialisations

**Key types:**
- `StandaloneSingleton<TSelf>` — Base class for standalone, globally accessible singletons.
- `SingletonComponent` — A serialisable, `IControlled`-compliant singleton managed by a single central `SingletonHub`.
- `SingletonHub` — Centralised manager for all `SingletonComponent`s, controlling lifecycle, scene dependencies, and initialization order.
- `SingletonInitOrderProvider` — Base class for defining project-specific initialization order for singletons.

**Project setup:**
1. Override `SingletonInitOrderProvider` to specify the order of singleton types in `InitialisationOrder` (Note: All included types here should extend from `SingletonComponent`)
2. Add `SingletonHub` instance to a scene.
3. Toggle on/off individual singletons for this scene. The list should automatically populate on refresh, after updating `SingletonInitOrderProvider.InitialisationOrder`, but you can use the "Force Update" button otherwise.
4. Optionally, provide scene-specific config data for any `SingletonComponent` by creating an instance of it (as a `ScriptableObject` asset) and assigning it to the `SingletonHub` scene instance.

---

## Time Controls

Utility layer for managing conditional time pauses, used for systems such as menus, cutscenes, or tutorials.
For an example, see `PauseTimeWhileEnabled`:
```csharp
public class PauseTimeWhileEnabled : MonoBehaviour
{
	void OnEnable()
	{
		MM.GameTimeState.StartPauseTime( this );
	}

	void OnDisable()
	{
		MM.GameTimeState.StopPauseTime( this );
	}
}
```

---

## Utilities

A collection of general-purpose helper classes for common gameplay and engine tasks.

Includes:
- `ApplicationUtils`
- `ComponentUtils`
- `DebugUtils`
- `LineUtils`
- `MathsUtils`

---

## Installation

Either download and add the package to a project manually, or use the **Package Manager** Git URL:
`https://github.com/JuniperLightningbug/MMUnityLibrary.git`

## License

This project is licensed under the [Apache License 2.0](./LICENSE).

© 2025 Martin Micklethwaite. All rights reserved.

**Important:** Use of this repository for AI training purposes is **not permitted** without express written permission.