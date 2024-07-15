Changelog                         {#changelog}
============

v1.1.3
- bugfix: drag items with "⊛" symbol (from inside of MF Window Editor) not working on Windows systems

v1.1.2
- bugfix: gear icon not being displayed in the editor, unicode character not displayed in some systems, changed by the circled asterisk

v1.1.1
- bugfix: hierarchy not being drawn correctly
- bugfix: CompositionSamples scene not correctly set

v1.1
(Important: This update has class name changes and classes that have the MonoBehaviour inheritance removed, to avoid erros on updating, make a Backup and Remove the v1.0 before importing the new ones)
- Major naming and code refactoring
- SetParent behavior improved with new settings
- GraphicRaycaster are cached, by default, to improve performance
- MFEvents moved from CanvasManager to MFSystemManager to agregate all events from all canvases
- classes that need an update call are executed by the MFSystemManager to improve performance
- added IUpdateEvent interface for classes that have an Update call executed by the MFSystemManager
- class EventsHandler added to handle events generated using the default MF inputs. Events handled in the CanvasManager moved to the EventsHandler to improve performance 
- I_MF_InputManager interface changed to InputManager abstract class
- added EventsHandler to the InputManager
- Raycaster made non Monobehaviour to reduce needed scene Components
- MFObjects detected by the Raycaster are sorted based on the CanvasManager sort order
- removed wrong inheritance from MFUtils
- bugfix: fixed error on exiting play mode when an MF Object that will be destroyed is open in the MF window 
- bigfix: fixed not finding canvas manager on setting up prefab
