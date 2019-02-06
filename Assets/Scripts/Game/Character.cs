using UnityEngine;
using System.Collections;
using DG.Tweening;

public class Character
{

    GameManager GM { get; set; }
    public Vector2Int OriginPos { get; set; }
    public string ResPath { get; set; }
    public float during = 0.08f;
    public GameObject MGameObject { get; set; }
    public Animator MAnimator { get; set; }


    /// <summary>
    /// 初始化角色
    /// </summary>
    public void Init()
    {

        GM = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();

        string _dbName = Dict.SqlDBName;
        OriginPos = new Vector2Int(System.Convert.ToInt32(Dict.GetAllDict()[Dict.SaveDBName]["z_save_in_map"]["heroTilePosX"][1]), System.Convert.ToInt32(Dict.GetAllDict()[Dict.SaveDBName]["z_save_in_map"]["heroTilePosY"][1]));
        ResPath = Dict.GetString(_dbName, "system_res", "res", 2);
        MGameObject = (GameObject)Object.Instantiate(Resources.Load(ResPath)) as GameObject;
        MGameObject.transform.SetParent(GM.GetMap().Path.transform);
        MGameObject.transform.position = new Vector3(OriginPos.x, OriginPos.y);
        MGameObject.AddComponent<DOTweenAnimation>();
        MAnimator = MGameObject.GetComponent<Animator>();
        MAnimator.SetInteger("direction", System.Convert.ToInt32(Dict.GetAllDict()[Dict.SaveDBName]["z_save_in_map"]["heroDir"][1]));
        MAnimator.SetLayerWeight(0, 1);
        //图形层级
        if (MGameObject.GetComponent<SpriteRenderer>())
        {

            MGameObject.GetComponent<SpriteRenderer>().sortingOrder = 3;

        }

        if (Application.isEditor)
        {

            MGameObject.tag = "Player";

        }

    }


    public IEnumerator MoveTo(Vector2Int _tar)
    {

        Transform _mTransform = MGameObject.transform;
        yield return new WaitUntil(() => !DOTween.IsTweening(_mTransform));

        //--------------方向--------------
        Vector2 _dirVector = _tar - new Vector2Int(Mathf.RoundToInt(_mTransform.position.x), Mathf.RoundToInt(_mTransform.position.y));
        if (_dirVector == Vector2.up)
        {

            MAnimator.SetInteger("direction", 1);
            SaveHeroInfo.heroDir = 1;

        }
        if (_dirVector == Vector2.down)
        {

            MAnimator.SetInteger("direction", 2);
            SaveHeroInfo.heroDir = 2;

        }
        if (_dirVector == Vector2.left)
        {

            MAnimator.SetInteger("direction", 3);
            SaveHeroInfo.heroDir = 3;

        }
        if (_dirVector == Vector2.right)
        {

            MAnimator.SetInteger("direction", 4);
            SaveHeroInfo.heroDir = 4;

        }
        MAnimator.SetLayerWeight(1, 1);
        //--------------------------------

        if (System.Math.Abs(_tar.x - _mTransform.position.x) < 0.2f)
        {

            _mTransform.DOMoveY(_tar.y, during);

        }
        if (System.Math.Abs(_tar.y - _mTransform.transform.position.y) < 0.2f)
        {

            _mTransform.DOMoveX(_tar.x, during);

        }

        //特殊情况 怪物魔法攻击
        Vector2Int[] _surroundPoint = {

            _tar + Vector2Int.up,
            _tar + Vector2Int.down,
            _tar + Vector2Int.left,
            _tar + Vector2Int.right,

           };

        string _dbName = Dict.SqlDBName;
        string _tabName = "map_obj";
        foreach (Vector2Int _pos in _surroundPoint)
        {

            if (_pos.x < 0 || _pos.x > 10 || _pos.y < 0 || _pos.y > 10)
            {

                continue;

            }

            int _mapObjId = GM.MPointGrid[_pos.x, _pos.y].MMapObjId;
            if (_mapObjId <= 0)
            {

                continue;

            }

            //是怪物
            int _typeId = Dict.GetInt(_dbName, _tabName, "type_id", _mapObjId);
            if (_typeId == 14)
            {

                int _miniTypeId = Dict.GetInt(_dbName, _tabName, "mini_type_id", _mapObjId);

                switch (_miniTypeId)
                {

                    case 25://魔法警卫
                        Vector2Int _secPos = _tar - _pos + _tar;
                        if (_secPos.x < 0 || _secPos.x > 10 || _secPos.y < 0 || _secPos.y > 10)
                        {

                            yield break;

                        }

                        _mapObjId = GM.MPointGrid[_secPos.x, _secPos.y].MMapObjId;
                        if (_mapObjId <= 0)
                        {

                            yield break;

                        }

                        //是对称的另一只怪物
                        _typeId = Dict.GetInt(_dbName, _tabName, "type_id", _mapObjId);
                        if (_typeId == 14)
                        {

                            if (Dict.GetInt(_dbName, _tabName, "mini_type_id", _mapObjId) == 25)
                            {

                                //达成被两只魔法警卫夹击的条件
                                GM.DoChange(1, -Mathf.RoundToInt(SaveHeroInfo.Attributes[1] / 2));
                                GM.DoVfx(GameManager.VfxType.Attack2);

                            }

                        }
                        yield break;
                    case 26://高级巫师
                        GM.DoChange(1, -200);
                        GM.DoVfx(GameManager.VfxType.Attack2);
                        yield break;
                    case 27://初级巫师
                        GM.DoChange(1, -100);
                        GM.DoVfx(GameManager.VfxType.Attack2);
                        yield break;

                }

            }


        }

    }


    public bool IsNotMoving()
    {

        return !DOTween.IsTweening(MGameObject.transform);

    }

}