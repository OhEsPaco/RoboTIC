// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Academy.HoloToolkit.Unity;
using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Llamado por PlaySpaceManager después de que los planos hayan sido generados a partir de un Spatial
/// Mapping Mesh. Esta clase puede colocar objetos con el componente Placeable de forma que queden
/// cercanos al usuario.
/// </summary>
public class SpaceCollectionManager : Singleton<SpaceCollectionManager>
{
    private List<GameObject> horizontalSurfaces;
    private List<GameObject> verticalSurfaces;

    /// <summary>
    /// Genera una colección de Placeables en el mundo y los coloca en planos adecuados.
    /// </summary>
    /// <param name="horizontalSurfaces">Planos horizontales.</param>
    /// <param name="verticalSurfaces">Planos verticales.</param>
    public void SetSurfaces(List<GameObject> horizontalSurfaces, List<GameObject> verticalSurfaces)
    {
        this.horizontalSurfaces = horizontalSurfaces;
        this.verticalSurfaces = verticalSurfaces;
    }

    public bool IsReady()
    {
        if (horizontalSurfaces != null && verticalSurfaces != null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Intenta colocar un objeto en el escenario.
    /// </summary>
    /// <param name="spaceObjectPrefabs">Objeto a colocar.</param>
    /// <returns>True si ha conseguido colocarlo, false si no.</returns>
    public bool PlaceItemInWorld(GameObject spaceObjectPrefabs)
    {
        List<GameObject> list = new List<GameObject>();
        list.Add(spaceObjectPrefabs);
        return PlaceItemsInWorld(list);
    }

    public bool PlaceItemsInWorld(List<GameObject> spaceObjectPrefabs)
    {
        if (horizontalSurfaces != null && verticalSurfaces != null)
        {
            List<GameObject> horizontalObjects = new List<GameObject>();
            List<GameObject> verticalObjects = new List<GameObject>();

            foreach (GameObject spacePrefab in spaceObjectPrefabs)
            {
                Placeable placeable = spacePrefab.GetComponent<Placeable>();
                if (placeable.PlacementSurface == PlacementSurfaces.Horizontal)
                {
                    horizontalObjects.Add(spacePrefab);
                }
                else
                {
                    verticalObjects.Add(spacePrefab);
                }
            }

            if (horizontalObjects.Count > 0)
            {
                CreateSpaceObjects(horizontalObjects, horizontalSurfaces, PlacementSurfaces.Horizontal);
            }

            if (verticalObjects.Count > 0)
            {
                CreateSpaceObjects(verticalObjects, verticalSurfaces, PlacementSurfaces.Vertical);
            }
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Crea una colección de objetos Placeable y los coloca en SurfacePlanes.
    /// </summary>
    /// <param name="spaceObjects">Lista de objetos que se quieren colocar.</param>
    /// <param name="surfaces">Lista de superficies en el mundo.</param>
    /// <param name="surfaceType">Tipo de las superficies (horizontal o vertical).</param>
    private void CreateSpaceObjects(List<GameObject> spaceObjects, List<GameObject> surfaces, PlacementSurfaces surfaceType)
    {
        List<int> UsedPlanes = new List<int>();

        // Sort the planes by distance to user.
        surfaces.Sort((lhs, rhs) =>
       {
           Vector3 headPosition = Camera.main.transform.position;
           Collider rightCollider = rhs.GetComponent<Collider>();
           Collider leftCollider = lhs.GetComponent<Collider>();

           // This plane is big enough, now we will evaluate how far the plane is from the user's head.
           // Since planes can be quite large, we should find the closest point on the plane's bounds to the
           // user's head, rather than just taking the plane's center position.
           Vector3 rightSpot = rightCollider.ClosestPointOnBounds(headPosition);
           Vector3 leftSpot = leftCollider.ClosestPointOnBounds(headPosition);

           return Vector3.Distance(leftSpot, headPosition).CompareTo(Vector3.Distance(rightSpot, headPosition));
       });

        foreach (GameObject item in spaceObjects)
        {
            int index = -1;
            Collider collider = item.GetComponent<Collider>();

            if (surfaceType == PlacementSurfaces.Vertical)
            {
                index = FindNearestPlane(surfaces, collider.bounds.size, UsedPlanes, true);
            }
            else
            {
                index = FindNearestPlane(surfaces, collider.bounds.size, UsedPlanes, false);
            }

            // If we can't find a good plane we will put the object floating in space.
            Vector3 position = Camera.main.transform.position + (Camera.main.transform.forward * 2.0f) + Camera.main.transform.right * (UnityEngine.Random.value - 1.0f) * 2.0f;
            Quaternion rotation = Quaternion.identity;

            // If we do find a good plane we can do something smarter.
            if (index >= 0)
            {
                UsedPlanes.Add(index);
                GameObject surface = surfaces[index];
                SurfacePlane plane = surface.GetComponent<SurfacePlane>();
                position = surface.transform.position + (plane.PlaneThickness * plane.SurfaceNormal);
                position = AdjustPositionWithSpatialMap(position, plane.SurfaceNormal);
                rotation = Camera.main.transform.localRotation;

                if (surfaceType == PlacementSurfaces.Vertical)
                {
                    // Vertical objects should face out from the wall.
                    rotation = Quaternion.LookRotation(surface.transform.forward, Vector3.up);
                }
                else
                {
                    // Horizontal objects should face the user.
                    rotation = Quaternion.LookRotation(Camera.main.transform.position);
                    rotation.x = 0f;
                    rotation.z = 0f;
                }
            }

            //Vector3 finalPosition = AdjustPositionWithSpatialMap(position, surfaceType);
            //GameObject spaceObject = Instantiate(item, position, rotation) as GameObject;
            item.transform.position = position;
            item.transform.rotation = rotation;
            item.transform.parent = gameObject.transform;
        }
    }

    /// <summary>
    /// Busca el plano más cercano que es capaz de contener un objeto.
    /// </summary>
    /// <param name="planes">Lista de planos.</param>
    /// <param name="minSize">Tamaño mínimo del plano.</param>
    /// <param name="startIndex">Indice de la lista de planos donde se empieza a buscar.</param>
    /// <param name="isVertical">Verdadero si se buscan superficies verticales.</param>
    /// <returns></returns>
    private int FindNearestPlane(List<GameObject> planes, Vector3 minSize, List<int> usedPlanes, bool isVertical)
    {
        int planeIndex = -1;

        for (int i = 0; i < planes.Count; i++)
        {
            if (usedPlanes.Contains(i))
            {
                continue;
            }

            Collider collider = planes[i].GetComponent<Collider>();
            if (isVertical && (collider.bounds.size.x < minSize.x || collider.bounds.size.y < minSize.y))
            {
                // This plane is too small to fit our vertical object.
                continue;
            }
            else if (!isVertical && (collider.bounds.size.x < minSize.x || collider.bounds.size.y < minSize.y))
            {
                // This plane is too small to fit our horizontal object.
                continue;
            }

            return i;
        }

        return planeIndex;
    }

    /// <summary>
    /// Ajusta la posición inicial del objeto si está siendo ocluido.
    /// </summary>
    /// <param name="position">Posición del objeto a ajustar.</param>
    /// <param name="surfaceNormal">Normal del plano en la que está apoyado el objeto.</param>
    /// <returns></returns>
    private Vector3 AdjustPositionWithSpatialMap(Vector3 position, Vector3 surfaceNormal)
    {
        Vector3 newPosition = position;
        RaycastHit hitInfo;
        float distance = 0.5f;

        // Check to see if there is a SpatialMapping mesh occluding the object at its current position.
        if (Physics.Raycast(position, surfaceNormal, out hitInfo, distance, SpatialMappingManager.Instance.LayerMask))
        {
            // If the object is occluded, reset its position.
            newPosition = hitInfo.point;
        }

        return newPosition;
    }
}