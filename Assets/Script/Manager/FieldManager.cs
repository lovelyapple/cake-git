using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldManager : SingleToneBase<FieldManager>
{
    [SerializeField] GameObject startPoint;
    [SerializeField] GameObject endPoint;
    [SerializeField] CharacterControllerJellyMesh mainCharacte;
	public bool IsPause = false;

}
