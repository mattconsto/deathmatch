using UnityEngine;
using System.Collections.Generic;

/*
  http://stackoverflow.com/a/28603035/3990396
*/
public class DecalDynamic {
	public static void BuildDecal(Decal decal) {
		MeshFilter filter = decal.GetComponent<MeshFilter>();
		if (filter == null) filter = decal.gameObject.AddComponent<MeshFilter>();
		if (decal.GetComponent<Renderer>() == null) decal.gameObject.AddComponent<MeshRenderer>();
		decal.GetComponent<Renderer>().material = decal.material;

		if (decal.material == null || decal.sprite == null) {
			filter.mesh = null;
			return;
		}

		GameObject[] affectedObjects = GetAffectedObjects(decal.GetBounds(), decal.affectedLayers);
		foreach (GameObject go in affectedObjects) {
			DecalBuilder.BuildDecalForObject(decal, go);
		}
		DecalBuilder.Push(decal.pushDistance);

		Mesh mesh = DecalBuilder.CreateMesh();
		if (mesh != null) {
			mesh.name = "DecalMesh";
			filter.mesh = mesh;
		}
	}

	public static GameObject[] GetAffectedObjects(Bounds bounds, LayerMask affectedLayers) {
		MeshRenderer[] renderers = (MeshRenderer[])GameObject.FindObjectsOfType<MeshRenderer>();
		List<GameObject> objects = new List<GameObject>();
		foreach (Renderer r in renderers) {
			if (!r.enabled) continue;
			// if (!IsLayerContains(affectedLayers, r.gameObject.layer)) continue;
			if (r.GetComponent<Decal>() != null) continue;

			if (bounds.Intersects(r.bounds)) {
				objects.Add(r.gameObject);
			}
		}
		return objects.ToArray();
	}
}