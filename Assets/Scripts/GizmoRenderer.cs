﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GizmoRenderer : MonoBehaviour {
	public string gizmoPath = "";

	public void OnDrawGizmos() {
		if(gizmoPath != "") Gizmos.DrawIcon(transform.position, gizmoPath, true);
		if(GetComponent<MeshCollider>() != null) Gizmos.DrawWireMesh(GetComponent<MeshCollider>().sharedMesh, transform.position, transform.rotation, transform.localScale);
	}
}