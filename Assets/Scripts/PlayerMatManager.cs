using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMatManager : MonoBehaviour
{
    public ParticleSystem trail, secondTrail;
    public ParticleSystem.MainModule trailMain, secondTrailMain;
    public SkinnedMeshRenderer mesh;
    public MeshRenderer bladeMesh;
    public Material blueBladeMat, blueHeroMat, redBladeMat, redHeroMat, yellowBladeMat, yellowHeroMat, crazyMat;
    void Start()
    {
        trailMain = trail.main;
        secondTrailMain = secondTrail.main;
    }

    public void GoBlue()
    {
        trailMain.startColor = Color.blue;
        secondTrailMain.startColor = Color.blue;
        mesh.material = blueHeroMat;
        bladeMesh.material = blueBladeMat;
    }
    public void GoRed()
    {
        trailMain.startColor = Color.red;
        secondTrailMain.startColor = Color.red;
        mesh.material = redHeroMat;
        bladeMesh.material = redBladeMat;
    }
    public void GoYellow()
    {
        trailMain.startColor = Color.yellow;
        secondTrailMain.startColor = Color.yellow;
        mesh.material = yellowHeroMat;
        bladeMesh.material = yellowBladeMat;
    }
    public void GoCrazy()
    {
        trailMain.startColor = Color.magenta;
        secondTrailMain.startColor = Color.magenta;
        mesh.material = crazyMat;
        bladeMesh.material = crazyMat;
    }
}
