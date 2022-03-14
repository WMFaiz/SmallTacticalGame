using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[ExecuteInEditMode]
public class Customization : MonoBehaviour
{
    private const string Bip001 = "Bip001";
    private const string Bip001Pelvis = "Bip001 Pelvis";
    private const string Bip001_Spine = "Bip001 Spine";
    private const string Backpack_container = "Backpack_container";
    private const string Bip001_L_Clavicle = "Bip001 L Clavicle";
    private const string Bip001_L_UpperArm = "Bip001 L UpperArm";
    private const string Bip001_L_Forearm = "Bip001 L Forearm";
    private const string Bip001_L_Hand = "Bip001 L Hand";
    private const string WeaponContainer_L = "WeaponContainer_L";
    private const string Bip001_R_Clavicle = "Bip001 R Clavicle";
    private const string Bip001_R_UpperArm = "Bip001 R UpperArm";
    private const string Bip001_R_Forearm = "Bip001 R Forearm";
    private const string Bip001_R_Hand = "Bip001 R Hand";
    private const string WeaponContainer = "WeaponContainer";
    private const string TS_Body = "TS_Body";
    private const string TS_Head = "TS_Head";
    private const string Bullets_Pool = "BulletsPool";
    private const string matrialsPath = "Assets/Toon_Soldiers/ToonSoldiers_2/models/Materials/Color";
    private const string Muzzle = "Muzzle";

    private List<GameObject> _BegList = new List<GameObject>();
    public Material[] ColorMaterials; 

    [HideInInspector]
    public List<GameObject> _LeftWeaponList = new List<GameObject>();
    [HideInInspector]
    public List<BulletEffect> _LBulletEffect = new List<BulletEffect>();

    [HideInInspector]
    public List<GameObject> _RightWeaponList = new List<GameObject>();
    [HideInInspector]
    public List<BulletEffect> _RBulletEffect = new List<BulletEffect>();
    [HideInInspector]
    public GameObject selectedMuzzle = null; 

    private List<GameObject> _HeadList = new List<GameObject>();
    private List<GameObject> _BodyList = new List<GameObject>();

    [HideInInspector]
    public GameObject BulletsPool;
    [HideInInspector]
    public List<Rigidbody> _RBBullets = new List<Rigidbody>();
    [HideInInspector]
    public List<GameObject> _Bullets = new List<GameObject>();

    private GameObject CurrentRWeapon, CurrentLWeapon, CurrentBeg = null;

    [Range(0, 5)]
    public int BegVariation = 0;

    [Range(0, 19)]
    public int LeftWeaponVariation = 0;

    [Range(0, 19)]
    public int RightWeaponVariation = 0;

    [Range(0, 14)]
    public int HeadVariation = 0;

    [Range(0, 8)]
    public int HeadColor = 0;

    [Range(0, 8)]
    public int BodyVariation = 0;

    [Range(0, 8)]
    public int BodyColor = 0;


    // Start is called before the first frame update
    private void Awake()
    {
        ColorMaterials = Resources.LoadAll(matrialsPath, typeof(Material)).Cast<Material>().ToArray();
        //Material blueMaterial = (Material)Resources.Load(matrialsPath + "ToonSoldiers_blue", typeof(Material));
        //Material brownMaterial = (Material)Resources.Load(matrialsPath + "ToonSoldiers_brown", typeof(Material));
        //Material greenMaterial = (Material)Resources.Load(matrialsPath + "ToonSoldiers_green", typeof(Material));
        //Material greyMaterial = (Material)Resources.Load(matrialsPath + "ToonSoldiers_grey", typeof(Material));
        //Material oliveMaterial = (Material)Resources.Load(matrialsPath + "ToonSoldiers_olive", typeof(Material));
        //Material redMaterial = (Material)Resources.Load(matrialsPath + "ToonSoldiers_red", typeof(Material));
        //Material whiteMaterial = (Material)Resources.Load(matrialsPath + "ToonSoldiers_white", typeof(Material));
        //Material yellowMaterial = (Material)Resources.Load(matrialsPath + "ToonSoldiers_yellow", typeof(Material));
        //ColorMaterials.Add(blackMaterial);
        //ColorMaterials.Add(blueMaterial);
        //ColorMaterials.Add(brownMaterial);
        //ColorMaterials.Add(greenMaterial);
        //ColorMaterials.Add(greyMaterial);
        //ColorMaterials.Add(oliveMaterial);
        //ColorMaterials.Add(redMaterial);
        //ColorMaterials.Add(whiteMaterial);
        //ColorMaterials.Add(yellowMaterial);

        GameObject _BegCustomization = transform.
                                        Find(Bip001).
                                        Find(Bip001Pelvis).
                                        Find(Bip001_Spine).
                                        Find(Backpack_container).
                                        gameObject;

        GameObject _LeftWeaponCustomization = transform.
                                                Find(Bip001).
                                                Find(Bip001Pelvis).
                                                Find(Bip001_Spine).
                                                Find(Bip001_L_Clavicle).
                                                Find(Bip001_L_UpperArm).
                                                Find(Bip001_L_Forearm).
                                                Find(Bip001_L_Hand).
                                                Find(WeaponContainer_L).
                                                gameObject;

        GameObject _RightWeaponCustomization = transform.
                                                Find(Bip001).
                                                Find(Bip001Pelvis).
                                                Find(Bip001_Spine).
                                                Find(Bip001_R_Clavicle).
                                                Find(Bip001_R_UpperArm).
                                                Find(Bip001_R_Forearm).
                                                Find(Bip001_R_Hand).
                                                Find(WeaponContainer).
                                                gameObject;

        //Begs
        foreach (Transform child in _BegCustomization.transform)
        {
            if (!_BegList.Contains(child.gameObject))
            {
                _BegList.Add(child.gameObject);
                child.gameObject.SetActive(false);
            }
        }

        _BegList.ForEach(delegate (GameObject go)
        {
            if (go.activeSelf)
            {
                CurrentBeg = go;
            }
        });

        //Weapon Left
        foreach (Transform child in _LeftWeaponCustomization.transform)
        {
            if (!_LeftWeaponList.Contains(child.gameObject))
            {
                _LeftWeaponList.Add(child.gameObject);
            }
        }

        //Weapon Right
        foreach (Transform child in _RightWeaponCustomization.transform)
        {
            if (!_RightWeaponList.Contains(child.gameObject))
            {
                _RightWeaponList.Add(child.gameObject);
            }
        }

        //Body And Head
        foreach (Transform child in transform)
        {
            if (child.name.Contains(TS_Body))
            {
                if (!_BodyList.Contains(child.gameObject))
                {
                    _BodyList.Add(child.gameObject);
                    child.gameObject.SetActive(false);
                }
            }
            else if (child.name.Contains(TS_Head))
            {
                if (!_HeadList.Contains(child.gameObject))
                {
                    _HeadList.Add(child.gameObject);
                    child.gameObject.SetActive(false);
                }
            }
        }
    }

    private void Start()
    {
        BegCustomization();
        LeftWeaponCustomization();
        RightWeaponCustomization();
        HeadCustomization();
        BodyCustomization();
    }

    private void Update()
    {
        BegCustomization();
        LeftWeaponCustomization();
        RightWeaponCustomization();
        HeadCustomization();
        BodyCustomization();
    }

    private void GetSelectedMuzzle()
    {
        GameObject getMuzzleGO = null;
        foreach (GameObject go in _RightWeaponList)
        {
            if (go.activeInHierarchy)
            {
                getMuzzleGO = go.transform.Find(Muzzle).gameObject;
                break;
            }
        }
        selectedMuzzle = getMuzzleGO;
    }

    private void BegCustomization()
    {
        if (BegVariation == _BegList.Count)
        {
            _BegList.ForEach(delegate (GameObject go) { go.SetActive(false); });
        }
        else if (BegVariation != _BegList.Count)
        {
            _BegList.ForEach(delegate (GameObject go) { go.SetActive(false); });
            _BegList[BegVariation].SetActive(true);
            CurrentBeg = _BegList[BegVariation];
        }
    }

    private void LeftWeaponCustomization()
    {
        if (LeftWeaponVariation == _LeftWeaponList.Count)
        {
            _LeftWeaponList.ForEach(delegate (GameObject go) { go.SetActive(false); });
        }
        else if (LeftWeaponVariation != _LeftWeaponList.Count)
        {
            if (CurrentLWeapon != _LeftWeaponList[LeftWeaponVariation]) 
            {
                _LeftWeaponList.ForEach(delegate (GameObject go) { go.SetActive(false); });
                _LeftWeaponList[LeftWeaponVariation].SetActive(true);
                CurrentLWeapon = _LeftWeaponList[LeftWeaponVariation];

                _Bullets.Clear();
                _RBBullets.Clear();
                _RBulletEffect.Clear();

                foreach (Transform child in CurrentLWeapon.transform)
                {
                    if (child.name.Contains(Bullets_Pool))
                    {
                        BulletsPool = child.gameObject;
                        foreach (Transform underChild in child.gameObject.transform)
                        {
                            _Bullets.Add(underChild.gameObject);
                            _RBBullets.Add(underChild.gameObject.GetComponent<Rigidbody>());
                            underChild.gameObject.SetActive(false);
                        }
                    }
                }

                _Bullets.ForEach(delegate (GameObject go)
                {
                    BulletEffect goWA = go.GetComponent<BulletEffect>();
                    if (!_RBulletEffect.Contains(goWA))
                    {
                        _RBulletEffect.Add(goWA);
                    }
                });
            }
        }
    }

    private void RightWeaponCustomization()
    {
        if (RightWeaponVariation == _RightWeaponList.Count)
        {
            _RightWeaponList.ForEach(delegate (GameObject go) { go.SetActive(false); });
        }
        else if (RightWeaponVariation != _RightWeaponList.Count)
        {
            if (CurrentRWeapon != _RightWeaponList[RightWeaponVariation])
            {
                _RightWeaponList.ForEach(delegate (GameObject go) { go.SetActive(false); });
                _RightWeaponList[RightWeaponVariation].SetActive(true);
                CurrentRWeapon = _RightWeaponList[RightWeaponVariation];

                _Bullets.Clear();
                _RBBullets.Clear();
                _RBulletEffect.Clear();

                foreach (Transform child in CurrentRWeapon.transform)
                {
                    if (child.name.Contains(Bullets_Pool))
                    {
                        BulletsPool = child.gameObject;
                        foreach (Transform underChild in child.gameObject.transform)
                        {
                            _Bullets.Add(underChild.gameObject);
                            _RBBullets.Add(underChild.gameObject.GetComponent<Rigidbody>());
                            underChild.gameObject.SetActive(false);
                        }
                    }
                }

                _Bullets.ForEach(delegate (GameObject go)
                {
                    BulletEffect goWA = go.GetComponent<BulletEffect>();
                    if (!_RBulletEffect.Contains(goWA))
                    {
                        _RBulletEffect.Add(goWA);
                    }
                });
            }
        }

        GetSelectedMuzzle();
    }

    private void HeadCustomization()
    {
        _HeadList.ForEach(delegate (GameObject go) { go.SetActive(false); });
        _HeadList[HeadVariation].SetActive(true);
    }

    private void BodyCustomization()
    {
        _BodyList.ForEach(delegate (GameObject go) { go.SetActive(false); });
        _BodyList[BodyVariation].SetActive(true);
    }
}
