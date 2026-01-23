using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMatManager : MonoBehaviour
{
    public ParticleSystem trail, secondTrail;
    public SkinnedMeshRenderer mesh;
    public MeshRenderer bladeMesh;
    public Material blueBladeMat, blueHeroMat, redBladeMat, redHeroMat, yellowBladeMat, yellowHeroMat, crazyMat;
    
    private ParticleSystem.MainModule _trailMain, _secondTrailMain;

    private void Start()
    {
        _trailMain = trail.main;
        _secondTrailMain = secondTrail.main;
    }

    public void GoBlue()
    {
        _trailMain.startColor = Color.blue;
        _secondTrailMain.startColor = Color.blue;
        mesh.material = blueHeroMat;
        bladeMesh.material = blueBladeMat;
    }
    public void GoRed()
    {
        _trailMain.startColor = Color.red;
        _secondTrailMain.startColor = Color.red;
        mesh.material = redHeroMat;
        bladeMesh.material = redBladeMat;
    }
    public void GoYellow()
    {
        _trailMain.startColor = Color.yellow;
        _secondTrailMain.startColor = Color.yellow;
        mesh.material = yellowHeroMat;
        bladeMesh.material = yellowBladeMat;
    }
    public void GoCrazy()
    {
        _trailMain.startColor = Color.magenta;
        _secondTrailMain.startColor = Color.magenta;
        mesh.material = crazyMat;
        bladeMesh.material = crazyMat;
    }
}
