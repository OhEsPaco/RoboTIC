using Microsoft.MixedReality.Toolkit.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public class MrtkPortingTools : MonoBehaviour
{
    [MenuItem("MRTK Porting Tools/Fix Buttons")]
    private static void FixButtons()
    {
        List<Type> typesToFind = new List<Type>() {
            typeof(SelectArrow),
            typeof(ConditionCardFrame),
            typeof(SelectedOutputMarker),
            typeof(EditorInstructionButton),
            typeof(EditorMenuButton),
            typeof(EditorSaveButton),
            typeof(EditorSurfacePoint), // Posiblemente haya que ponerlos en runtime
            typeof(EditorTool),
            typeof(EditorToolFeedback),
            typeof(MoveCursor),
            typeof(MessageScreenButton),
            typeof(LoopCounter),
            typeof(RoadButton),
            typeof(GenericButton),
        };

        foreach (Type typeToFind in typesToFind)
        {
            Debug.Log("Searching all instances of " + typeToFind + "...");
            var foundOccurences = GetAllOccurencesInScene(typeToFind);
            Debug.Log("Found " + foundOccurences.Length + " instances");

            System.Reflection.MethodInfo onSelectMethod = typeToFind.GetMethod("OnSelect");

            foreach (dynamic foundInstance in foundOccurences)
            {
                Interactable addedInteractable = foundInstance.gameObject.GetComponent<Interactable>();

                if (addedInteractable == null)
                {
                    addedInteractable = foundInstance.gameObject.AddComponent<Interactable>();
                    UnityEditor.Events.UnityEventTools.AddPersistentListener(addedInteractable.OnClick, System.Delegate.CreateDelegate(typeof(UnityAction), foundInstance, onSelectMethod));
                }
            }
        }
    }

    private static object[] GetAllOccurencesInScene(Type typeToFind)
    {
        ArrayList foundObjects = new ArrayList();
        GameObject[] rootObjects = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();

        foreach (GameObject rootObject in rootObjects)
        {
            var componentsInChildren = rootObject.GetComponentsInChildren(typeToFind, true);

            if (componentsInChildren != null && componentsInChildren.Length > 0)
            {
                foreach (var componentInChildren in componentsInChildren)
                {
                    foundObjects.Add(componentInChildren);
                }
            }
        }

        return foundObjects.ToArray();
    }
}