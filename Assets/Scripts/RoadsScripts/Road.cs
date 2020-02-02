using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static RoadConstants;

public abstract class Road : MonoBehaviour
{
    [SerializeField] private RoadType roadType = RoadType.Undetermined;
    [SerializeField] private RoadIO[] inputsAndOutputs;

    private void Start()
    {
        //Pedimos los inputs
        inputsAndOutputs = GetComponentsInChildren<RoadInput>();

        //Pedimos los outputs en un array diferente
        RoadOutput[] outputs = GetComponentsInChildren<RoadOutput>();

        //Hacemos que el array final aumente su longitud para acomodar a los outputs

        Array.Resize(ref inputsAndOutputs, inputsAndOutputs.Length + outputs.Length);

        //Copiamos los outputs
        Array.Copy(outputs, 0, inputsAndOutputs, inputsAndOutputs.Length - outputs.Length, outputs.Length);
    }

    public void SetThisOutputActive(RoadIO rio)
    {
        foreach (RoadIO thisIO in inputsAndOutputs)
        {
            if (thisIO == rio)
            {
                thisIO.Active = true;
            }
            else
            {
                thisIO.Active = false;
            }
        }
    }

    public RoadType RoadType { get => roadType; }
    public RoadIO[] InputsAndOutputs { get => inputsAndOutputs; }

    public List<RoadIO> Inputs()
    {
        return ReturnByIO(InputOutput.Input);
    }

    public List<RoadIO> Outputs()
    {
        return ReturnByIO(InputOutput.Output);
    }

    public List<RoadIO> ReturnByIO(in InputOutput isIO)
    {
        List<RoadIO> thisIos = new List<RoadIO>();
        foreach (RoadIO roadIo in inputsAndOutputs)
        {
            if (roadIo.IsInputOrOutput() == isIO)
            {
                thisIos.Add(roadIo);
            }
        }
        return thisIos;
    }

    public List<RoadIO> ReturnByIOAndDirection(in InputOutput isIO, in PointingTo pointsTo)
    {
        List<RoadIO> thisIos = new List<RoadIO>();
        foreach (RoadIO roadIo in inputsAndOutputs)
        {
            if (roadIo.IsInputOrOutput() == isIO && roadIo.PointsTo == pointsTo)
            {
                thisIos.Add(roadIo);
            }
        }
        return thisIos;
    }

    public List<RoadIO> ReturnByIOAndDirectionAndType(in InputOutput isIO, in PointingTo pointsTo, in IOType ioType)
    {
        List<RoadIO> thisIos = new List<RoadIO>();
        foreach (RoadIO roadIo in inputsAndOutputs)
        {
            if (roadIo.IsInputOrOutput() == isIO && roadIo.PointsTo == pointsTo && roadIo.IoType == ioType)
            {
                thisIos.Add(roadIo);
            }
        }
        return thisIos;
    }

    public List<RoadIO> ReturnByDirection(in PointingTo pointsTo)
    {
        List<RoadIO> thisIos = new List<RoadIO>();
        foreach (RoadIO roadIo in inputsAndOutputs)
        {
            if (roadIo.PointsTo == pointsTo)
            {
                thisIos.Add(roadIo);
            }
        }
        return thisIos;
    }

    public abstract void ExecuteAction(in Actions action, in int[] arguments);

    private void OnDrawGizmos()
    {
        Handles.Label(transform.position, roadType.ToString("g"));
    }
}