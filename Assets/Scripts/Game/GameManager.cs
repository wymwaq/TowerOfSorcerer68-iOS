using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;


public class GameManager : MonoBehaviour
{

    public Common MyCommon { get; set; }


    public Character MyPlayer { get; set; }


    public UI MyUI { get; set; }
    public bool IsTriggering { get; set; }
    public bool IsAutoMoving { get; set; }


    Map _map;


    //当前点击的item Obj物体的Pos
    public AStarPoint ActivateAStarPoint { get; set; }


    //存储路径点
    public List<AStarPoint> MPathPosList;
    //存储基本触控cube
    public Dictionary<Vector2, Cube> MBaseCubeList = new Dictionary<Vector2, Cube>();
    //全部位置信息
    public AStarPoint[,] MPointGrid { get; set; }


    //寻路的起始位置
    public AStarPoint MStartPos { get; set; }
    //寻路的终点位置
    public AStarPoint MEndPos { get; set; }

    //PropertyChange的所有商店除取消按钮的Id集
    public List<int> ShopPropertyChangeIds { get; set; }


    //每一秒发生位移
    float MTime { get; set; }
    float MTimer { get; set; }


    public Map GetMap()
    {

        return _map;

    }


    public enum VfxType
    {

        HpText, Attack1, Attack2

    }


    void Start()
    {

        Time.timeScale = 1;
        //DBDataBase.DictBase.PosYConvert("Sql");
        //DBDataBase.DictBase.PosYConvert("System");
        //DBDataBase.DictBase.PosYConvert("Wall");
        //for (int _i = 0; _i < 10; _i++)
        //{

        //    DBDataBase.DictBase.PosYConvert("SaveDb_" + _i);

        //}
        if (GameObject.FindWithTag("Common") == null)
        {

            SceneManager.LoadScene(0);
            return;

        }
        MyCommon = GameObject.FindWithTag("Common").GetComponent<Common>();
        MyCommon.PlayBGM(8);

        Application.SetStackTraceLogType(LogType.Assert, StackTraceLogType.None);
        Application.SetStackTraceLogType(LogType.Error, StackTraceLogType.None);
        Application.SetStackTraceLogType(LogType.Exception, StackTraceLogType.None);
        Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.None);
        Application.SetStackTraceLogType(LogType.Warning, StackTraceLogType.None);
        _map = new Map();
        MyUI = new UI();
        MyPlayer = new Character();
        MTime = MyPlayer.during;
        MTimer = 0.0f;

        //相机初始化
        InitCamera(Dict.MapWidth, Dict.MapHeight);
        //地图基本数值初始化
        _map.Init();
        //寻路系统初始化
        InitAStar();
        //Player和位置 初始化
        MyPlayer.Init();
        MStartPos = MPointGrid[MyPlayer.OriginPos.x, MyPlayer.OriginPos.y];
        //UI数值初始化
        MyUI.Init();
        //Save类初始化
        SaveHeroInfo.Init();
        SaveHeroInfo.HeroTilePosX = MStartPos.MPositionX;
        SaveHeroInfo.HeroTilePosY = MStartPos.MPositionY;
        //UI数值初始化 - 楼层
        MyUI.FloorText.text = MyUI.FloorText.text.Replace(MyUI.FloorText.text, "F" + Dict.GetString(Dict.SqlDBName, "map", "name", SaveHeroInfo.CurMapId));

        _map.CreateCurMapAllObjs();

        //PropertyChange的所有商店(除取消按钮)的Id集初始化
        ShopPropertyChangeIds = new List<int>
        {

            100,102,104,107,109,111,114,116,118,121,123,125,128,130,132,149,151,153

        };

        for (int _index = 0; _index < ShopPropertyChangeIds.Count; _index++)
        {

            int _changeId = ShopPropertyChangeIds[_index];
            int _next_changeId = Dict.GetInt(Dict.SqlDBName, "property_change", "next_change_id", _changeId);
            while (_next_changeId > 0)
            {

                ShopPropertyChangeIds.Add(_next_changeId);
                _changeId = _next_changeId;
                _next_changeId = Dict.GetInt(Dict.SqlDBName, "property_change", "next_change_id", _changeId);

            }

        }

    }


    void Update()
    {

        //if (!MyUI.IsAllUIClosed())
        //{

        //    return;

        //}
        MTimer += Time.deltaTime;
        if (MTimer >= MTime)
        {

            MTimer = 0;
            Walk();

        }

        if (Input.GetKeyDown(KeyCode.O))
        {

            //Debug.Log(MStartPos.MPosition);
            //List<int> aaa = new List<int> { 41, 15, 46, 49, 52, 53, 56, 54, 55, 58, 59, 61, 62, 66, 68, 43 };
            //for (int i = 0; i < aaa.Count; i++)
            //{

            //    //int _changeId = _dict.GetInt(_dict.SqlDBName, "map_obj_type", "change_id", aaa[i]);
            //DoPropertyChange(aaa[i]);

            //}

            for (int i = 1; i <= 11; i++)
            {

                DoChange(i, 100);

            }

            for (int i = 0; i < SaveHeroInfo.Props.Count; i++)
            {

                SaveHeroInfo.Props[i] = 0;

            }
            SaveHeroInfo.RefreshUI();

            SaveHeroInfo.ReachFloorIdLimit = new Vector2Int(0, 68);

            SaveHeroInfo.Attributes[12] = 1;

            SaveHeroInfo.Attributes[13] = 1;

        }

        if (Input.GetKeyDown(KeyCode.Q))
        {

            Debug.Log(MStartPos.MPosition);
            foreach (AStarPoint grid in MPointGrid)
            {

                if (grid.MGameObject)
                {

                    Debug.Log(grid.MPosition + "/" + grid.MGameObject.name + "/" + grid.MType + "/" + grid.MMapObjId);

                }

            }
            Debug.Log("增加障碍");

            int x = MStartPos.MPositionX;
            int y = MStartPos.MPositionY;
            MPointGrid[x - 1, y].MType = PointType.Obstacle;
            MPointGrid[x - 1, y + 1].MType = PointType.Obstacle;
            MPointGrid[x, y + 1].MType = PointType.Obstacle;
            MPointGrid[x + 1, y + 1].MType = PointType.Obstacle;
            MPointGrid[x + 1, y].MType = PointType.Obstacle;
            MPointGrid[x + 1, y - 1].MType = PointType.Obstacle;
            MPointGrid[x, y - 1].MType = PointType.Obstacle;
            MPointGrid[x - 1, y - 1].MType = PointType.Obstacle;

            Debug.Log("加载障碍完毕");

        }

        if (Input.GetKeyDown(KeyCode.E))
        {

            int x = MStartPos.MPositionX;
            int y = MStartPos.MPositionY;
            MPointGrid[x - 1, y].MType = PointType.Through;
            MPointGrid[x - 1, y + 1].MType = PointType.Through;
            MPointGrid[x, y + 1].MType = PointType.Through;
            MPointGrid[x + 1, y + 1].MType = PointType.Through;
            MPointGrid[x + 1, y].MType = PointType.Through;
            MPointGrid[x + 1, y - 1].MType = PointType.Through;
            MPointGrid[x, y - 1].MType = PointType.Through;
            MPointGrid[x - 1, y - 1].MType = PointType.Through;

        }

    }


    /// <summary>
    /// 刷新指定位置的map_obj状态
    /// </summary>
    /// <param name="_mapobjid">Mapobjid.</param>
    public void UpdateSwitch(int _mapobjid)
    {

        string _dbName = Dict.SqlDBName;
        //64层 MapId65 火眼金睛升级处 特殊情况 机关门关闭的情况
        switch (_mapobjid)
        {

            case 1860:
                if (Dict.GetInt(_dbName, "map_obj", "value", _mapobjid) == 1)
                {

                    Dict.SetInt(_dbName, "map_obj", "value", 1857, 1);
                    MPointGrid[Dict.GetInt(_dbName, "map_obj", "pos_x", 1857), Dict.GetInt(_dbName, "map_obj", "pos_y", 1857)].UpdateSelf();

                }
                break;
            case 1861:
                if (Dict.GetInt(_dbName, "map_obj", "value", _mapobjid) == 1)
                {

                    Dict.SetInt(_dbName, "map_obj", "value", 1858, 1);
                    MPointGrid[Dict.GetInt(_dbName, "map_obj", "pos_x", 1858), Dict.GetInt(_dbName, "map_obj", "pos_y", 1858)].UpdateSelf();

                }
                break;
            case 1862:
                if (Dict.GetInt(_dbName, "map_obj", "value", _mapobjid) == 1)
                {

                    Dict.SetInt(_dbName, "map_obj", "value", 1859, 1);
                    MPointGrid[Dict.GetInt(_dbName, "map_obj", "pos_x", 1859), Dict.GetInt(_dbName, "map_obj", "pos_y", 1859)].UpdateSelf();

                }
                break;

        }

        //id为map_obj_change的id
        for (int _mapObjChangeId = 1; _mapObjChangeId <= Dict.GetColCount(_dbName, "map_obj_change"); _mapObjChangeId++)
        {

            if (_mapobjid == Dict.GetInt(_dbName, "map_obj_change", "map_obj_id", _mapObjChangeId))
            {

                //---------------------------------
                //Type 0 : 条件  1 : 结果
                if (Dict.GetInt(_dbName, "map_obj_change", "type", _mapObjChangeId) == 0)
                {

                    List<int> _switchIdsCondition = new List<int>();
                    List<int> _switchIdsResult = new List<int>();

                    //获得全部关联的条件Id-------------
                    int _switchBeforeId = Dict.GetInt(_dbName, "map_obj_change", "switch_before", _mapObjChangeId);
                    //先退回到初始的id
                    while (_switchBeforeId > 0)
                    {

                        _mapObjChangeId = _switchBeforeId;
                        _switchBeforeId = Dict.GetInt(_dbName, "map_obj_change", "switch_before", _mapObjChangeId);

                    }
                    _switchIdsCondition.Add(_mapObjChangeId);
                    int _switchAfterId = Dict.GetInt(_dbName, "map_obj_change", "switch_after", _mapObjChangeId);
                    while (_switchAfterId > 0)
                    {

                        _mapObjChangeId = _switchAfterId;
                        _switchAfterId = Dict.GetInt(_dbName, "map_obj_change", "switch_after", _mapObjChangeId);
                        _switchIdsCondition.Add(_mapObjChangeId);

                    }

                    foreach (int _conditionId in _switchIdsCondition)
                    {

                        int _tarValue = Dict.GetInt(_dbName, "map_obj_change", "value", _conditionId);
                        int _mapObjId = Dict.GetInt(_dbName, "map_obj_change", "map_obj_id", _conditionId);
                        int _curValue = Dict.GetInt(_dbName, "map_obj", "value", _mapObjId);
                        if (_curValue != _tarValue)
                        {

                            Debug.Log("条件不足");
                            return;

                        }

                    }

                    //获得全部关联的结局Id-------------
                    //获得ResultId
                    _mapObjChangeId = Dict.GetInt(_dbName, "map_obj_change", "result_id", _switchIdsCondition[_switchIdsCondition.Count - 1]);
                    int _resultId = Dict.GetInt(_dbName, "map_obj_change", "result_id", _mapObjChangeId);
                    _switchIdsResult.Add(_mapObjChangeId);
                    while (_resultId > 0)
                    {

                        _mapObjChangeId = _resultId;
                        _resultId = Dict.GetInt(_dbName, "map_obj_change", "result_id", _mapObjChangeId);
                        _switchIdsResult.Add(_mapObjChangeId);

                    }

                    foreach (int _switchIdResult in _switchIdsResult)
                    {

                        int _tarValue = Dict.GetInt(_dbName, "map_obj_change", "value", _switchIdResult);
                        int _mapObjId = Dict.GetInt(_dbName, "map_obj_change", "map_obj_id", _switchIdResult);
                        Dict.SetInt(_dbName, "map_obj", "value", _mapObjId, _tarValue);
                        int _x = Dict.GetInt(_dbName, "map_obj", "pos_x", _mapObjId);
                        int _y = Dict.GetInt(_dbName, "map_obj", "pos_y", _mapObjId);
                        //0 显示 1 消失
                        if (_tarValue == 0)
                        {

                            MPointGrid[_x, _y].Clear();
                            MPointGrid[_x, _y].MMapObjId = _mapObjId;
                            MPointGrid[_x, _y].UpdateSelf();

                        }
                        else
                        {

                            MPointGrid[_x, _y].Clear();

                        }

                    }
                    return;

                }
                //---------------------------------

            }

        }

    }


    void DoSwitchOffByStoryId(int _switchoffid)
    {

        string _dbName = Dict.SqlDBName;
        string _tabName = "map_obj";
        for (int _mapObjId = 1; _mapObjId <= Dict.GetColCount(_dbName, _tabName); _mapObjId++)
        {

            if (Dict.GetInt(_dbName, _tabName, "switch_off", _mapObjId) == _switchoffid)
            {

                //设为不存在
                Dict.SetInt(_dbName, _tabName, "value", _mapObjId, 1);
                //刷新在本地图更新的物品
                if (Dict.GetInt(_dbName, _tabName, "map_id", _mapObjId) == SaveHeroInfo.CurMapId)
                {

                    int _x = Dict.GetInt(_dbName, _tabName, "pos_x", _mapObjId);
                    int _y = Dict.GetInt(_dbName, _tabName, "pos_y", _mapObjId);
                    if (MPointGrid[_x, _y].MMapObjId > 0)
                    {

                        MPointGrid[_x, _y].UpdateSelf();

                    }

                }
                //SaveMapObjById(_mapObjId);

            }

        }

    }
    void DoSwitchOnByStoryId(int _switchonid)
    {

        string _dbName = Dict.SqlDBName;
        string _tabName = "map_obj";

        //遍历map_obj
        for (int _mapObjId = 1; _mapObjId <= Dict.GetColCount(_dbName, _tabName); _mapObjId++)
        {

            //找到匹配的 switch_on  == storyid 的物件id
            if (Dict.GetInt(_dbName, _tabName, "switch_on", _mapObjId) == _switchonid)
            {

                //设为存在
                Dict.SetInt(_dbName, _tabName, "value", _mapObjId, 0);
                //刷新在当前层更新的物品
                if (Dict.GetInt(_dbName, _tabName, "map_id", _mapObjId) == SaveHeroInfo.CurMapId)
                {

                    int _x = Dict.GetInt(_dbName, _tabName, "pos_x", _mapObjId);
                    int _y = Dict.GetInt(_dbName, _tabName, "pos_y", _mapObjId);
                    MPointGrid[_x, _y].MMapObjId = _mapObjId;
                    MPointGrid[_x, _y].UpdateSelf();

                }
                //SaveMapObjById(_mapObjId);

            }

        }

    }


    IEnumerator AutoMove(int _automoveid)
    {

        IsAutoMoving = true;
        //第一段automove
        if (MPathPosList != null)
        {

            MPathPosList.Clear();
            //MPathPosList = AStarAlgorithm.GetInsatnce.FindPath(MStartPos, MEndPos);

        }

        AStarAlgorithm.GetInsatnce.ClearGrid();

        string dbName = Dict.SqlDBName;
        string tabName = "auto_move";

        //标记AutoMove事件为以触发 0 : 存在 1 : 消失
        Dict.SetInt(dbName, tabName, "value", _automoveid, 1);
        int _x = Dict.GetInt(dbName, tabName, "hero_pos_x", _automoveid);
        int _y = Dict.GetInt(dbName, tabName, "hero_pos_y", _automoveid);
        MPointGrid[_x, _y].Clear();

        //初始地点
        Vector2Int startPos = new Vector2Int
        (

            Dict.GetInt(dbName, tabName, "start_pos_x", _automoveid),
            Dict.GetInt(dbName, tabName, "start_pos_y", _automoveid)

        );

        //目标地点
        Vector2Int endPos = new Vector2Int
        (

            Dict.GetInt(dbName, tabName, "end_pos_x", _automoveid),
            Dict.GetInt(dbName, tabName, "end_pos_y", _automoveid)

        );

        yield return new WaitUntil(MyPlayer.IsNotMoving);
        yield return new WaitForSeconds(0.2f);

        MyPlayer.MGameObject.transform.position = new Vector3(startPos.x, startPos.y);
        FindPath(endPos.x, endPos.y);
        StartCoroutine(MyPlayer.MoveTo(endPos));
        yield return new WaitUntil(MyPlayer.IsNotMoving);
        yield return new WaitForSeconds(0.2f);
        //MStartPos.MPosition = endPos;

        //清除automoveid
        //MPointGrid[(int)MStartPos.MPosition.x, (int)MStartPos.MPosition.y].MAutoMoveId = 0;

        //下一段 story 功能未完成
        //int _nextAutoMoveId = _dict.GetInt("Sql", "auto_move", "next_auto_move_id", _automoveid);
        //next auto move（如果存在） 
        //while (_nextAutoMoveId > 0)
        //{
        //    _automoveid = _nextAutoMoveId
        //}

        //判定是否有storyId 是否触发的story
        int _endStoryId = Dict.GetInt(Dict.SqlDBName, "auto_move", "end_story_id", _automoveid);
        if (_endStoryId > 0)
        {

            StartCoroutine(DoStory(_endStoryId));

        }
        else
        {

            IsAutoMoving = false;

        }

    }


    void Walk()
    {

        if (MPathPosList == null || MPathPosList.Count <= 1 || !MyPlayer.IsNotMoving())
        {

            MyPlayer.MAnimator.SetLayerWeight(1, 0);
            return;

        }

        //出发点 = MPathPosList最后一格
        //MStartPos = MPathPosList[MPathPosList.Count - 1];

        //Color color = MStartPos.MGameObject.GetComponent<MeshRenderer>().materials[0].GetColor("_ShaderColor");

        //MPathPosList移除最后一个 = 移除出发点的位置
        MPathPosList.Remove(MStartPos);

        Destroy(MStartPos.MGameObject);
        MStartPos.MGameObject = null;

        //出发点 = MPathPosList最后一格（已经移除了初始的出发点）
        MStartPos = MPathPosList[MPathPosList.Count - 1];
        //MStartPos.MGameObject.GetComponent<MeshRenderer>().materials[0].SetColor("_ShaderColor", color);

        //人物移动表现
        StartCoroutine(MyPlayer.MoveTo(new Vector2Int(MStartPos.MPositionX, MStartPos.MPositionY)));

        //检测AutoMove点
        //如果当前所在点是autoMove点 auto_move_obj的位置
        int _autoMoveId = MPointGrid[MStartPos.MPositionX, MStartPos.MPositionY].MAutoMoveId;

        if (_autoMoveId <= 0)
        {

            //移动到最后一格
            if (MStartPos == MPathPosList[0])
            {

                if (ActivateAStarPoint != null)
                {

                    IsTriggering = true;
                    StartCoroutine(TriggerEvent());

                }
                else
                {


                }
                SaveHeroInfo.HeroTilePosX = MStartPos.MPositionX;
                SaveHeroInfo.HeroTilePosY = MStartPos.MPositionY;

            }

        }
        //存在_autoMoveId > 0
        else
        {

            StartCoroutine(AutoMove(_autoMoveId));

        }

    }


    void InitAStar()
    {

        AStarAlgorithm.MGridWidth = Dict.MapWidth;
        AStarAlgorithm.MGridHeight = Dict.MapHeight;
        AStarAlgorithm.Dir8 = false;// 四方向行走
        AStarAlgorithm.PathRoot = _map.Path.transform;
        MPointGrid = AStarAlgorithm.GetInsatnce.mPointGrid;

    }


    /// <summary>
    /// 初始化相机
    /// </summary>
    /// <param name="_mapw">地图格子数——横.</param>
    /// <param name="_maph">地图格子数——纵.</param>
    void InitCamera(int _mapw, int _maph)
    {

        Camera cam = Camera.main;
        cam.backgroundColor = Color.black;
        cam.transform.position = new Vector3(Mathf.FloorToInt(_mapw * 0.5f), _maph * 0.61f, -2);
        cam.orthographicSize = 9.7f;

        cam.pixelRect.Set(0, 0, cam.pixelHeight, cam.pixelWidth);

    }


    /// <summary>
    /// 触发传送点
    /// </summary>
    /// <param name="_warppointid">Warppointid.</param>
    void TriggerWarpPoint(int _warppointid)
    {

        string _dbName = Dict.SqlDBName;
        string _tabName = "map_warp_point";
        //传送到指定点
        if (MPathPosList != null)
        {

            MPathPosList.Clear();

        }

        ActivateAStarPoint = null;
        AStarAlgorithm.GetInsatnce.ClearGrid();

        int _tarMapId = Dict.GetInt(_dbName, _tabName, "tar_map_id", _warppointid);
        Vector2Int _tarPos = new Vector2Int
        (

            Dict.GetInt(_dbName, _tabName, "tar_pos_x", _warppointid),
            Dict.GetInt(_dbName, _tabName, "tar_pos_y", _warppointid)

        );
        int _tarHeroDir = Dict.GetInt(_dbName, _tabName, "tar_hero_dir", _warppointid);

        MStartPos = MPointGrid[_tarPos.x, _tarPos.y];
        MyPlayer.MGameObject.transform.position = new Vector3(_tarPos.x, _tarPos.y);
        MyPlayer.MAnimator.SetInteger("direction", _tarHeroDir);

        SaveHeroInfo.CurMapId = _tarMapId;
        if (SaveHeroInfo.CurMapId >= 12)
        {

            Dict.SetInt(Dict.SqlDBName, "system", "title_bg", 1, 1);

        }
        if (SaveHeroInfo.CurMapId >= 22)
        {

            Dict.SetInt(Dict.SqlDBName, "system", "title_bg", 1, 2);

        }

        if (SaveHeroInfo.CurMapId >= 42)
        {

            Dict.SetInt(Dict.SqlDBName, "system", "title_bg", 1, 3);

        }

        if (SaveHeroInfo.CurMapId >= 62)
        {

            Dict.SetInt(Dict.SqlDBName, "system", "title_bg", 1, 4);

        }

        Debug.Log("进入MapId" + SaveHeroInfo.CurMapId);
        MyUI.FloorText.text = MyUI.FloorText.text.Replace(MyUI.FloorText.text, "F" + Dict.GetString(Dict.SqlDBName, "map", "name", SaveHeroInfo.CurMapId));
        _map.CreateCurMapAllObjs();
        //音效
        MyCommon.MusicSource[6].Play();

        //最高到达层刷新
        SaveHeroInfo.RefreshUI();
        CloseUIBox();

    }


    /// <summary>
    /// 到达地图
    /// </summary>
    /// <param name="_mapid">Mapid.</param>
    /// <param name="_isjumpfloor">If set to <c>true</c> _isjumpfloor.</param>
    public void EnterMapId(int _mapid, bool _isjumpfloor = false)
    {

        string _dbName = Dict.SqlDBName;
        string _tabName = "map_warp_point";
        //传送到指定点
        if (MPathPosList != null)
        {

            MPathPosList.Clear();

        }

        ActivateAStarPoint = null;
        AStarAlgorithm.GetInsatnce.ClearGrid();

        //最终得到的
        int _tarWarpPointId = 1;
        for (int _warpPointId = 1; _warpPointId <= Dict.GetColCount(_dbName, _tabName); _warpPointId++)
        {

            int _curMapId = Dict.GetInt(_dbName, _tabName, "cur_map_id", _warpPointId);
            if (_curMapId == _mapid)
            {

                int _tarMapId = Dict.GetInt(_dbName, _tabName, "tar_map_id", _warpPointId);
                //当前楼层小于目标楼层  且传送点为回上一层的点
                if (SaveHeroInfo.CurMapId <= _curMapId && _tarMapId < _curMapId)
                {

                    _tarWarpPointId = _warpPointId;
                    break;

                }
                if (SaveHeroInfo.CurMapId > _curMapId && _tarMapId > _curMapId)
                {

                    _tarWarpPointId = _warpPointId;
                    break;

                }

            }

        }

        int _x = Dict.GetInt(_dbName, _tabName, "cur_pos_x", _tarWarpPointId);
        int _y = Dict.GetInt(_dbName, _tabName, "cur_pos_y", _tarWarpPointId);
        MStartPos = MPointGrid[_x, _y];
        MyPlayer.MGameObject.transform.position = new Vector3(_x, _y);

        SaveHeroInfo.CurMapId = _mapid;
        MyUI.FloorText.text = MyUI.FloorText.text.Replace(MyUI.FloorText.text, "F" + Dict.GetString(Dict.SqlDBName, "map", "name", SaveHeroInfo.CurMapId));
        _map.CreateCurMapAllObjs();

        //最高到达层刷新
        SaveHeroInfo.RefreshUI();

        if (_isjumpfloor)
        {

            return;

        }
        CloseUIBox();

    }


    /// <summary>
    /// 攻击事件 true:攻击成功 false:攻击失败
    /// </summary>
    /// <param name="minitypeid">Minitypeid.</param>
    bool AttackEvent(int minitypeid)
    {

        string _dbName = Dict.SqlDBName;
        int _monHp = Dict.GetInt(_dbName, "monster", "hp", minitypeid);
        int _monAt = Dict.GetInt(_dbName, "monster", "atk", minitypeid);
        int _monDf = Dict.GetInt(_dbName, "monster", "def", minitypeid);
        int _monExp = Dict.GetInt(_dbName, "monster", "exp", minitypeid);
        int _monGold = Dict.GetInt(_dbName, "monster", "gold", minitypeid);
        int _heroAt = SaveHeroInfo.Attributes[2];
        //佩戴了十字架
        if (SaveHeroInfo.Props[2] == 0)
        {

            if (minitypeid == 11 || minitypeid == 13 || minitypeid == 15)
            {

                _heroAt *= 2;

            }

        }
        //Hp    =>  SaveHI.Attributes[1];
        //At    =>  SaveHI.Attributes[2];
        //Df    =>  SaveHI.Attributes[3];
        //Gold  =>  SaveHI.Attributes[4];
        //Exp   =>  SaveHI.Attributes[5];

        //伤害值float
        float _hurtValue = _heroAt <= _monDf ? 0 : _monHp / (SaveHeroInfo.Attributes[2] - _monDf);
        //SaveHI.Attributes[1] -= _dict.IsInteger(hurtValue.ToString()) ? (int)((hurtValue - 1) * (monAt - SaveHI.Attributes[3])) : Mathf.CeilToInt(hurtValue) * (monAt - SaveHI.Attributes[3]);
        //实际伤害值int
        int _changeNum = Dict.IsInteger(_hurtValue.ToString()) ? System.Convert.ToInt32((_hurtValue - 1) * (_monAt - SaveHeroInfo.Attributes[3])) : Mathf.CeilToInt(_hurtValue) * (_monAt - SaveHeroInfo.Attributes[3]);
        //Hero At <= Monster DF 或 hp <= 实际伤害值
        if (_heroAt <= _monDf || SaveHeroInfo.Attributes[1] <= _changeNum)
        {

            if (MPointGrid[ActivateAStarPoint.MPositionX, ActivateAStarPoint.MPositionY].MType == PointType.TempThrough)
            {

                MPointGrid[ActivateAStarPoint.MPositionX, ActivateAStarPoint.MPositionY].MType = PointType.Obstacle;

            }
            Debug.Log("打不过");
            return false;

        }

        //Hero扣血
        DoChange(1, -_changeNum);
        //带了幸运金币
        if (SaveHeroInfo.Props[7] == 0)
        {

            _monGold *= 2;

        }
        //Gold赏金
        DoChange(4, _monGold);
        Debug.Log("受到" + _changeNum + "点伤害！");

        //攻击特效表现
        DoVfx(VfxType.Attack1);
        return true;

    }


    ///// <summary>
    ///// 保存 Map Obj 数据 by Id.
    ///// </summary>
    ///// <param name="mapobjid">Mapobjid.</param>
    //public void SaveMapObjById(int mapobjid)
    //{

    //    //记录至存档dict
    //    //_dict.SetInt(_dict.SaveDBName, "z_save_map_obj", "value", mapobjid, 1);
    //    //RefreshMapObj(mapobjid);

    //}


    /// <summary>
    /// 触发obj
    /// </summary>
    /// <param name="_mapobjid">Mapobjid.</param>
    public void TriggerObj(int _mapobjid)
    {

        string _dbName = Dict.SqlDBName;
        string _tabName = "map_obj";
        //记录物品的原位置
        //Vector2Int _pos = new Vector2Int(ActivateAStarPoint.MPositionX, ActivateAStarPoint.MPositionY);
        Vector2Int _pos = new Vector2Int(Dict.GetInt(_dbName, _tabName, "pos_x", _mapobjid), Dict.GetInt(_dbName, _tabName, "pos_y", _mapobjid));

        //目标的type_id
        int _typeId = Dict.GetInt(_dbName, _tabName, "type_id", _mapobjid);
        int _miniTypeId;
        int _changeId;
        int _storyId;
        int _selectId;

        //消除物件
        AStarPoint tarPoint = MPointGrid[_pos.x, _pos.y];

        switch (_typeId)
        {

            //需要消耗道具
            case 1:
            case 2:
            case 3:
                _storyId = Dict.GetInt(_dbName, _tabName, "story_id", _mapobjid);
                if (_storyId > 0)
                {

                    StartCoroutine(DoStory(_storyId));

                }
                _changeId = Dict.GetInt(_dbName, "map_obj_type", "change_id", _typeId);
                if (DoPropertyChange(_changeId))
                {

                    DestroyMapObj(tarPoint);

                    //音效
                    MyCommon.MusicSource[3].Play();

                    //39楼机关黄门 筋斗云获得处
                    if (_mapobjid >= 1450 && _mapobjid <= 1458)
                    {

                        List<int> otherIds = new List<int>
                        {

                            1450,1452,1453,1454,1456,1457,1458

                        };

                        List<int> tarIds = new List<int>
                        {

                            1451,1455

                        };

                        foreach (int _otherId in otherIds)
                        {

                            //有任意一个otherId消失 0 存在 1 消失
                            int _otherValue = Dict.GetInt(_dbName, _tabName, "value", _otherId);
                            if (_otherValue == 1)
                            {

                                //筋斗云消失
                                Dict.SetInt(_dbName, _tabName, "value", 1061, 1);
                                return;

                            }

                        }

                        foreach (int _tarId in tarIds)
                        {

                            //有任意一个tarId存在  0 存在 1 消失
                            int _tarValue = Dict.GetInt(_dbName, _tabName, "value", _tarId);
                            if (_tarValue == 0)
                            {

                                return;

                            }

                        }
                        //触发Story77
                        //将中间的黄门转成筋斗云
                        StartCoroutine(DoStory(77));

                    }
                    UpdateSwitch(_mapobjid);

                }
                break;

            case 7://上楼梯左
            case 8://下楼梯
            case 18://上楼梯右
                _storyId = Dict.GetInt(_dbName, _tabName, "story_id", _mapobjid);
                if (_storyId > 0)
                {

                    StartCoroutine(DoStory(_storyId));

                }
                UpdateSwitch(_mapobjid);

                Debug.Log("楼梯");
                break;

            case 13://Npc    story id | select id
                _storyId = Dict.GetInt(_dbName, _tabName, "story_id", _mapobjid);
                if (_storyId > 0)
                {

                    StartCoroutine(DoStory(_storyId));

                }

                _miniTypeId = Dict.GetInt(_dbName, _tabName, "mini_type_id", _mapobjid);
                _storyId = Dict.GetInt(_dbName, "npc", "story_id", _miniTypeId);
                _selectId = Dict.GetInt(_dbName, "npc", "select_id", _miniTypeId);
                if (_storyId > 0)
                {

                    StartCoroutine(DoStory(_storyId));
                    UpdateSwitch(_mapobjid);

                }

                if (_selectId > 0)
                {

                    DoHeroSelect(_selectId);

                }

                //音效
                MyCommon.MusicSource[5].Play();
                break;

            case 14://Monster
            case 71://Boss怪
                _storyId = Dict.GetInt(_dbName, _tabName, "story_id", _mapobjid);
                if (_storyId > 0)
                {

                    StartCoroutine(DoStory(_storyId));

                }

                _miniTypeId = Dict.GetInt(_dbName, _tabName, "mini_type_id", _mapobjid);
                if (AttackEvent(_miniTypeId))
                {

                    DestroyMapObj(tarPoint);
                    UpdateSwitch(_mapobjid);

                }

                break;

            case 16://装饰物
                _storyId = Dict.GetInt(_dbName, _tabName, "story_id", _mapobjid);
                if (_storyId > 0)
                {

                    StartCoroutine(DoStory(_storyId));

                }
                UpdateSwitch(_mapobjid);

                Debug.Log("装饰物");
                break;

            case 42://机关门
                _storyId = Dict.GetInt(_dbName, _tabName, "story_id", _mapobjid);
                if (_storyId > 0)
                {

                    StartCoroutine(DoStory(_storyId));

                }
                UpdateSwitch(_mapobjid);

                Debug.Log("机关门");
                break;

            case 65://铁栅栏
                _storyId = Dict.GetInt(_dbName, _tabName, "story_id", _mapobjid);
                if (_storyId > 0)
                {

                    StartCoroutine(DoStory(_storyId));

                }
                UpdateSwitch(_mapobjid);

                Debug.Log("铁栅栏");
                break;

            case 41://楼层跳跃器
            case 15://火眼金睛
            case 46://十字架
            case 49://圣水
            case 52://黄金钥匙
            case 53://向下飞行器
            case 56://向上飞行器
            case 54://幸运金币
            case 55://炸药
            case 58://屠龙匕
            case 59://避水珠
            case 61://芭蕉扇
            case 62://猴毛
            case 66://铁镐
            case 68://地震卷轴
            case 43://筋斗云
                _storyId = Dict.GetInt(_dbName, _tabName, "story_id", _mapobjid);
                if (_storyId > 0)
                {

                    StartCoroutine(DoStory(_storyId));

                }

                //普通道具 非怪物、Npc、Boss、装饰物
                _changeId = Dict.GetInt(_dbName, "map_obj_type", "change_id", _typeId);
                DoPropertyChange(_changeId);
                DestroyMapObj(tarPoint);

                //音效
                MyCommon.MusicSource[1].Play();
                break;

            case 39://铁剑
                _storyId = Dict.GetInt(_dbName, _tabName, "story_id", _mapobjid);
                if (_storyId > 0)
                {

                    StartCoroutine(DoStory(_storyId));

                }

                _changeId = Dict.GetInt(_dbName, "map_obj_type", "change_id", _typeId);
                DoPropertyChange(_changeId);
                DestroyMapObj(tarPoint);

                MyUI.WeaponUI.GetComponent<Image>().sprite = MyUI.EquipUIImages[1];
                MyUI.WeaponUI.GetComponent<Button>().onClick.RemoveAllListeners();
                MyUI.WeaponUI.GetComponent<Button>().onClick.AddListener(() => DoMessage("武器：铁剑"));

                //音效
                MyCommon.MusicSource[1].Play();
                break;

            case 44://银剑
                _storyId = Dict.GetInt(_dbName, _tabName, "story_id", _mapobjid);
                if (_storyId > 0)
                {

                    StartCoroutine(DoStory(_storyId));

                }

                _changeId = Dict.GetInt(_dbName, "map_obj_type", "change_id", _typeId);
                DoPropertyChange(_changeId);
                DestroyMapObj(tarPoint);

                MyUI.WeaponUI.GetComponent<Image>().sprite = MyUI.EquipUIImages[3];
                MyUI.WeaponUI.GetComponent<Button>().onClick.RemoveAllListeners();
                MyUI.WeaponUI.GetComponent<Button>().onClick.AddListener(() => DoMessage("武器：银剑"));

                //音效
                MyCommon.MusicSource[1].Play();
                break;

            case 47://骑士剑
                _storyId = Dict.GetInt(_dbName, _tabName, "story_id", _mapobjid);
                if (_storyId > 0)
                {

                    StartCoroutine(DoStory(_storyId));

                }

                _changeId = Dict.GetInt(_dbName, "map_obj_type", "change_id", _typeId);
                DoPropertyChange(_changeId);
                DestroyMapObj(tarPoint);

                MyUI.WeaponUI.GetComponent<Image>().sprite = MyUI.EquipUIImages[5];
                MyUI.WeaponUI.GetComponent<Button>().onClick.RemoveAllListeners();
                MyUI.WeaponUI.GetComponent<Button>().onClick.AddListener(() => DoMessage("武器：骑士剑"));

                //音效
                MyCommon.MusicSource[1].Play();
                break;

            case 51://圣剑
                _storyId = Dict.GetInt(_dbName, _tabName, "story_id", _mapobjid);
                if (_storyId > 0)
                {

                    StartCoroutine(DoStory(_storyId));

                }

                _changeId = Dict.GetInt(_dbName, "map_obj_type", "change_id", _typeId);
                DoPropertyChange(_changeId);
                DestroyMapObj(tarPoint);

                MyUI.WeaponUI.GetComponent<Image>().sprite = MyUI.EquipUIImages[7];
                MyUI.WeaponUI.GetComponent<Button>().onClick.RemoveAllListeners();
                MyUI.WeaponUI.GetComponent<Button>().onClick.AddListener(() => DoMessage("武器：圣剑"));

                //音效
                MyCommon.MusicSource[1].Play();
                break;

            case 60://神圣剑
                _storyId = Dict.GetInt(_dbName, _tabName, "story_id", _mapobjid);
                if (_storyId > 0)
                {

                    StartCoroutine(DoStory(_storyId));

                }

                _changeId = Dict.GetInt(_dbName, "map_obj_type", "change_id", _typeId);
                DoPropertyChange(_changeId);
                DestroyMapObj(tarPoint);

                MyUI.WeaponUI.GetComponent<Image>().sprite = MyUI.EquipUIImages[10];
                MyUI.WeaponUI.GetComponent<Button>().onClick.RemoveAllListeners();
                MyUI.WeaponUI.GetComponent<Button>().onClick.AddListener(() => DoMessage("武器：神圣剑"));

                //音效
                MyCommon.MusicSource[1].Play();
                break;

            case 64://金箍棒
                _storyId = Dict.GetInt(_dbName, _tabName, "story_id", _mapobjid);
                if (_storyId > 0)
                {

                    StartCoroutine(DoStory(_storyId));

                }

                _changeId = Dict.GetInt(_dbName, "map_obj_type", "change_id", _typeId);
                DoPropertyChange(_changeId);
                DestroyMapObj(tarPoint);

                MyUI.WeaponUI.GetComponent<Image>().sprite = MyUI.EquipUIImages[11];
                MyUI.WeaponUI.GetComponent<Button>().onClick.RemoveAllListeners();
                MyUI.WeaponUI.GetComponent<Button>().onClick.AddListener(() => DoMessage("武器：金箍棒"));

                //音效
                MyCommon.MusicSource[1].Play();
                break;

            case 40://铁盾
                _storyId = Dict.GetInt(_dbName, _tabName, "story_id", _mapobjid);
                if (_storyId > 0)
                {

                    StartCoroutine(DoStory(_storyId));

                }

                _changeId = Dict.GetInt(_dbName, "map_obj_type", "change_id", _typeId);
                DoPropertyChange(_changeId);
                DestroyMapObj(tarPoint);

                MyUI.ArmorUI.GetComponent<Image>().sprite = MyUI.EquipUIImages[2];
                MyUI.ArmorUI.GetComponent<Button>().onClick.RemoveAllListeners();
                MyUI.ArmorUI.GetComponent<Button>().onClick.AddListener(() => DoMessage("防具：铁盾"));

                //音效
                MyCommon.MusicSource[1].Play();
                break;

            case 45://银盾
                _storyId = Dict.GetInt(_dbName, _tabName, "story_id", _mapobjid);
                if (_storyId > 0)
                {

                    StartCoroutine(DoStory(_storyId));

                }

                _changeId = Dict.GetInt(_dbName, "map_obj_type", "change_id", _typeId);
                DoPropertyChange(_changeId);
                DestroyMapObj(tarPoint);

                MyUI.ArmorUI.GetComponent<Image>().sprite = MyUI.EquipUIImages[4];
                MyUI.ArmorUI.GetComponent<Button>().onClick.RemoveAllListeners();
                MyUI.ArmorUI.GetComponent<Button>().onClick.AddListener(() => DoMessage("防具：银盾"));

                //音效
                MyCommon.MusicSource[1].Play();
                break;

            case 48://骑士盾
                _storyId = Dict.GetInt(_dbName, _tabName, "story_id", _mapobjid);
                if (_storyId > 0)
                {

                    StartCoroutine(DoStory(_storyId));

                }

                _changeId = Dict.GetInt(_dbName, "map_obj_type", "change_id", _typeId);
                DoPropertyChange(_changeId);
                DestroyMapObj(tarPoint);

                MyUI.ArmorUI.GetComponent<Image>().sprite = MyUI.EquipUIImages[6];
                MyUI.ArmorUI.GetComponent<Button>().onClick.RemoveAllListeners();
                MyUI.ArmorUI.GetComponent<Button>().onClick.AddListener(() => DoMessage("防具：骑士盾"));

                //音效
                MyCommon.MusicSource[1].Play();
                break;

            case 50://圣盾
                _storyId = Dict.GetInt(_dbName, _tabName, "story_id", _mapobjid);
                if (_storyId > 0)
                {

                    StartCoroutine(DoStory(_storyId));

                }

                _changeId = Dict.GetInt(_dbName, "map_obj_type", "change_id", _typeId);
                DoPropertyChange(_changeId);
                DestroyMapObj(tarPoint);

                MyUI.ArmorUI.GetComponent<Image>().sprite = MyUI.EquipUIImages[8];
                MyUI.ArmorUI.GetComponent<Button>().onClick.RemoveAllListeners();
                MyUI.ArmorUI.GetComponent<Button>().onClick.AddListener(() => DoMessage("防具：圣盾"));

                //音效
                MyCommon.MusicSource[1].Play();
                break;

            case 57://神圣盾
                _storyId = Dict.GetInt(_dbName, _tabName, "story_id", _mapobjid);
                if (_storyId > 0)
                {

                    StartCoroutine(DoStory(_storyId));

                }

                _changeId = Dict.GetInt(_dbName, "map_obj_type", "change_id", _typeId);
                DoPropertyChange(_changeId);
                DestroyMapObj(tarPoint);

                MyUI.ArmorUI.GetComponent<Image>().sprite = MyUI.EquipUIImages[9];
                MyUI.ArmorUI.GetComponent<Button>().onClick.RemoveAllListeners();
                MyUI.ArmorUI.GetComponent<Button>().onClick.AddListener(() => DoMessage("防具：神圣盾"));

                //音效
                MyCommon.MusicSource[1].Play();
                break;

            case 63://虎皮裙
                _storyId = Dict.GetInt(_dbName, _tabName, "story_id", _mapobjid);
                if (_storyId > 0)
                {

                    StartCoroutine(DoStory(_storyId));

                }

                _changeId = Dict.GetInt(_dbName, "map_obj_type", "change_id", _typeId);
                DoPropertyChange(_changeId);
                DestroyMapObj(tarPoint);

                MyUI.ArmorUI.GetComponent<Image>().sprite = MyUI.EquipUIImages[12];
                MyUI.ArmorUI.GetComponent<Button>().onClick.RemoveAllListeners();
                MyUI.ArmorUI.GetComponent<Button>().onClick.AddListener(() => DoMessage("防具：虎皮裙"));

                //音效
                MyCommon.MusicSource[1].Play();
                break;

            //普通可拾取道具
            case 4:
            case 5:
            case 6:
            case 9:
            case 10:
            case 11:
            case 12:
            case 19:
            case 20:
            case 21:
            case 22:
            case 23:
            case 24:
            case 25:
            case 26:
            case 27:
            case 28:
            case 29:
            case 30:
            case 31:
            case 32:
            case 33:
            case 34:
            case 35:
            case 36:
            case 37:
            case 38:
                _storyId = Dict.GetInt(_dbName, _tabName, "story_id", _mapobjid);
                if (_storyId > 0)
                {

                    StartCoroutine(DoStory(_storyId));

                }

                _changeId = Dict.GetInt(_dbName, "map_obj_type", "change_id", _typeId);
                DoPropertyChange(_changeId);
                DestroyMapObj(tarPoint);
                UpdateSwitch(_mapobjid);

                //音效 为钥匙
                if (_typeId == 4 || _typeId == 5 || _typeId == 6)
                {

                    MyCommon.MusicSource[2].Play();

                }
                else
                {

                    MyCommon.MusicSource[1].Play();

                }
                break;

            case 67://变色后的墙
                _storyId = Dict.GetInt(_dbName, _tabName, "story_id", _mapobjid);
                if (_storyId > 0)
                {

                    StartCoroutine(DoStory(_storyId));

                }
                UpdateSwitch(_mapobjid);

                Debug.Log("变色后的墙");
                break;

            case 69://岩浆
                _storyId = Dict.GetInt(_dbName, _tabName, "story_id", _mapobjid);
                if (_storyId > 0)
                {

                    StartCoroutine(DoStory(_storyId));

                }

                //0 存在  1 消失
                if (SaveHeroInfo.Props[10] == 0)
                {

                    DestroyMapObj(tarPoint);

                }
                UpdateSwitch(_mapobjid);

                Debug.Log("岩浆");
                break;

            case 70://火焰山
                _storyId = Dict.GetInt(_dbName, _tabName, "story_id", _mapobjid);
                if (_storyId > 0)
                {

                    StartCoroutine(DoStory(_storyId));

                }
                UpdateSwitch(_mapobjid);

                Debug.Log("火焰山");
                break;

            case 72://触碰变色
                Dict.SetInt(_dbName, _tabName, "value", _mapobjid, 1);
                for (int _mapObjId = 1; _mapObjId <= Dict.GetColCount(_dbName, _tabName); _mapObjId++)
                {

                    int _oriMapId = Dict.GetInt(_dbName, _tabName, "map_id", _mapobjid);
                    int _mapId = Dict.GetInt(_dbName, _tabName, "map_id", _mapObjId);
                    if (_mapId == _oriMapId)
                    {

                        if (_mapObjId != _mapobjid)
                        {

                            Vector2Int _tarPos = new Vector2Int(Dict.GetInt(_dbName, _tabName, "pos_x", _mapObjId), Dict.GetInt(_dbName, _tabName, "pos_y", _mapObjId));
                            if (_tarPos == _pos)
                            {

                                //可触碰变色的72号墙消失
                                Dict.SetInt(_dbName, _tabName, "value", _mapobjid, 1);
                                //67号新墙出现
                                Dict.SetInt(_dbName, _tabName, "value", _mapObjId, 0);
                                MPointGrid[_pos.x, _pos.y].UpdateSelf();

                            }

                        }

                    }

                }


                _storyId = Dict.GetInt(_dbName, _tabName, "story_id", _mapobjid);
                if (_storyId > 0)
                {

                    StartCoroutine(DoStory(_storyId));

                }
                UpdateSwitch(_mapobjid);

                Debug.Log("触碰变色");
                break;

            case 73://主要是暗墙
                _storyId = Dict.GetInt(_dbName, _tabName, "story_id", _mapobjid);
                if (_storyId > 0)
                {

                    StartCoroutine(DoStory(_storyId));

                }
                DestroyMapObj(tarPoint);
                UpdateSwitch(_mapobjid);

                Debug.Log("暗墙");
                break;

            case 74://彩蛋暗墙
                _storyId = Dict.GetInt(_dbName, _tabName, "story_id", _mapobjid);
                if (_storyId > 0)
                {

                    StartCoroutine(DoStory(_storyId));

                }
                UpdateSwitch(_mapobjid);

                Debug.Log("彩蛋暗墙");
                break;

            case 75://彩蛋挡墙
                _storyId = Dict.GetInt(_dbName, _tabName, "story_id", _mapobjid);
                if (_storyId > 0)
                {

                    StartCoroutine(DoStory(_storyId));

                }
                UpdateSwitch(_mapobjid);

                Debug.Log("彩蛋挡墙");
                break;

        }
        //前进一格 到达原物品的位置
        if (MPointGrid[_pos.x, _pos.y].MGameObject == null) FindPath(_pos.x, _pos.y);

    }


    IEnumerator TriggerEvent()
    {

        if (IsAutoMoving)
        {

            ActivateAStarPoint = null;
            IsTriggering = false;
            yield break;

        }

        yield return new WaitUntil(MyPlayer.IsNotMoving);
        yield return new WaitForSeconds(0.2f);

        if (ActivateAStarPoint != null)
        {

            int _warpPointId = ActivateAStarPoint.MWarpPointId;
            //目标Obj的map_obj id
            int _mapObjId = ActivateAStarPoint.MMapObjId;

            //是warp_point
            if (_mapObjId == 0)
            {

                if (_warpPointId > 0)
                {

                    TriggerWarpPoint(_warpPointId);

                }

            }
            //是Object
            else
            {

                if (_warpPointId <= 0)
                {

                    TriggerObj(_mapObjId);

                }

            }
            ActivateAStarPoint = null;

        }
        IsTriggering = false;

    }


    public void FindPath(int mTargetX, int mTargetY)
    {

        if (MPathPosList != null)
        {

            MPathPosList.Clear();

        }

        AStarAlgorithm.GetInsatnce.ClearGrid();

        ////网格点对象重新刷新了  需要使用网格来索引到点 mPathPosList存储的点是之前的AStarPoint
        MEndPos = MPointGrid[mTargetX, mTargetY];
        MStartPos = MPointGrid[MStartPos.MPositionX, MStartPos.MPositionY];

        MPathPosList = AStarAlgorithm.GetInsatnce.FindPath(MStartPos, MEndPos);

        //如果是item 移除路径最后一格

        if (MPointGrid[mTargetX, mTargetY].MType == PointType.TempThrough)
        {

            //待定
            ////被包围的情况
            //if (MPathPosList.Count == 1 && MPathPosList[0] == MStartPos)
            //{

            //    MPathPosList[0] = MStartPos;
            //    //StartCoroutine(TriggerEvent());
            //    return;

            //}

            MPathPosList.RemoveAt(0);

            //如果已在临格 无需行走
            if (MPathPosList.Count > 0 && MPathPosList[0] == MStartPos)
            {

                StartCoroutine(TriggerEvent());

            }

        }

    }


    /// <summary>
    /// true: 条件不足 false: 条件足够 继续执行
    /// </summary>
    /// <returns><c>true</c>, if check fail was ised, <c>false</c> otherwise.</returns>
    /// <param name="_id">PropertyChangeId.</param>
    bool CheckIsFail(int _id)
    {

        int _changeType = Dict.GetInt(Dict.SqlDBName, "property_change", "change_type", _id);
        int _changeNum = Dict.GetInt(Dict.SqlDBName, "property_change", "change_num", _id);

        switch (_changeType)
        {

            case 4://Gold
                if (MyUI.IsShop(_id, true))
                {

                    _changeNum = -SaveHeroInfo.ShopPrice;

                }
                if (SaveHeroInfo.Attributes[4] < Mathf.Abs(_changeNum)) return true;
                break;

            case 6://KeyRed
                if (SaveHeroInfo.Attributes[9] <= 0) return true;
                break;

            case 7://KeyBlue
                if (SaveHeroInfo.Attributes[10] <= 0) return true;
                break;

            case 8://KeyYellow
                if (SaveHeroInfo.Attributes[11] <= 0) return true;
                break;

        }

        ////是商店升级选项  非取消
        //if (MyUI.IsShop(_id, true))
        //{

        //    SaveHeroInfo.PurchaseCount++;

        //}
        return false;

    }


    /// <summary>
    /// 执行PropertyChange改变 true:执行成功 false:执行失败
    /// </summary>
    /// <returns><c>true</c>, if property change was done, <c>false</c> otherwise.</returns>
    /// <param name="_id">PropertyChange Id</param>
    bool DoPropertyChange(int _id)
    {

        string _dbName = Dict.SqlDBName;
        //PropertyChange 关联id合集
        List<int> _ids = GetAllLinkPropertyChangeIds(_id);
        //第三级  0: change_type  1: change_id  2: change_num  3:story_id
        List<List<int>> _idValues = new List<List<int>>();

        for (int _index = 0; _index < _ids.Count; _index++)
        {

            int _isCheck = Dict.GetInt(_dbName, "property_change", "is_check", _ids[_index]);
            if (_isCheck > 0)
            {

                if (CheckIsFail(_ids[_index]))
                {

                    Debug.Log("条件不足");
                    return false;

                }

            }

            int _changeType = Dict.GetInt(_dbName, "property_change", "change_type", _ids[_index]);
            int _changeId = Dict.GetInt(_dbName, "property_change", "change_id", _ids[_index]);
            int _changeNum = Dict.GetInt(_dbName, "property_change", "change_num", _ids[_index]);
            int _storyId = Dict.GetInt(_dbName, "property_change", "story_id", _ids[_index]);

            _idValues.Add(new List<int>());
            _idValues[_index].Add(_changeType);
            _idValues[_index].Add(_changeId);
            _idValues[_index].Add(_changeNum);
            _idValues[_index].Add(_storyId);

        }

        //DoChange
        for (int i = 0; i < _idValues.Count; i++)
        {

            //第三级  0: change_type  1: change_id  2: change_num  3:story_id
            switch (_idValues[i][0])
            {

                case 1://攻击
                    if (_idValues[i][1] == 1)//为成比增加
                    {

                        _idValues[i][2] = SaveHeroInfo.Attributes[2] * _idValues[i][2] / 100;

                    }
                    DoChange(2, _idValues[i][2]);
                    break;

                case 2://防御
                    if (_idValues[i][1] == 1)//为成比增加
                    {

                        _idValues[i][2] = SaveHeroInfo.Attributes[3] * _idValues[i][2] / 100;

                    }
                    DoChange(3, _idValues[i][2]);
                    break;

                case 3://血量
                    if (_idValues[i][1] == 1)//为成比增加
                    {

                        _idValues[i][2] = SaveHeroInfo.Attributes[1] * _idValues[i][2] / 100;

                    }
                    DoChange(1, _idValues[i][2]);
                    break;

                case 4://金币
                    //是商店
                    if (MyUI.IsShop(_id, true))
                    {

                        DoChange(4, -SaveHeroInfo.ShopPrice);

                    }
                    else
                    {

                        DoChange(4, _idValues[i][2]);

                    }
                    break;

                case 5://经验
                    DoChange(5, _idValues[i][2]);
                    break;

                case 6://红钥匙
                    DoChange(9, _idValues[i][2]);
                    break;

                case 7://蓝钥匙
                    DoChange(10, _idValues[i][2]);
                    break;

                case 8://黄钥匙
                    DoChange(11, _idValues[i][2]);
                    break;

                case 9://等级
                    DoChange(6, _idValues[i][2]);
                    break;

                case 10://攻击装备
                    DoChange(7, _idValues[i][2]);
                    break;

                case 11://防御装备
                    DoChange(8, _idValues[i][2]);
                    break;

                case 12://其他道具
                    //ChangeId  0:显示 1:消失
                    SaveHeroInfo.Props[_idValues[i][1] - 1] = 0;
                    SaveHeroInfo.RefreshUI();
                    break;

            }

            //改变属性后 如果还有StoryId
            if (_idValues[i][3] > 0)
            {

                StartCoroutine(DoStory(_idValues[i][3]));

            }

        }
        //MyUI.CloseBox();
        return true;

    }


    public void DoChange(int _realId, int _num)
    {

        //HP变化
        if (_realId == 1)
        {

            //数值跳动变化提示
            if (_num > 0)
            {

                //加血
                GameObject.FindWithTag("HpChangeNum").GetComponent<Text>().text = "+" + Mathf.Abs(_num) + "HP";
                GameObject.FindWithTag("HpChangeNum").GetComponent<Text>().color = Color.green;
                DoVfx(VfxType.HpText);


            }
            else
            {

                //扣血
                if (Mathf.Abs(_num) == 0)
                {

                    GameObject.FindWithTag("HpChangeNum").GetComponent<Text>().text = "兵不血刃！";
                    GameObject.FindWithTag("HpChangeNum").GetComponent<Text>().color = Color.green;

                }
                else
                {

                    GameObject.FindWithTag("HpChangeNum").GetComponent<Text>().text = "-" + Mathf.Abs(_num) + "HP";
                    GameObject.FindWithTag("HpChangeNum").GetComponent<Text>().color = Color.red;

                }
                DoVfx(VfxType.HpText);

            }

        }

        SaveHeroInfo.Attributes[_realId] += _num;
        // 更新主面板属性 attrsvalue
        MyUI.AttrsValue[_realId].text = SaveHeroInfo.Attributes[_realId].ToString();

    }


    /// <summary>
    /// 特效播放
    /// </summary>
    /// <param name="vfxType">Vfx type.</param>
    public void DoVfx(VfxType vfxType)
    {

        Vector2 _pos = new Vector2();
        switch (vfxType)
        {

            case VfxType.HpText:     //0 Hp动画 文字特效
                if (MStartPos != null)
                {

                    _pos = new Vector2(MStartPos.MPositionX, MStartPos.MPositionY);

                }
                else if (ActivateAStarPoint != null)
                {

                    _pos = new Vector2(ActivateAStarPoint.MPositionX, ActivateAStarPoint.MPositionY);

                }
                else
                {

                    _pos = Vector2.zero;

                }
                Instantiate(Resources.Load<Object>("VFXs/HpChangeAnim"), _pos + new Vector2(0, 0.5f), Quaternion.Euler(-90, 0, 0));
                break;

            case VfxType.Attack1:     //1 攻击1 主角
                if (ActivateAStarPoint != null)
                {

                    _pos = new Vector2(ActivateAStarPoint.MPositionX, ActivateAStarPoint.MPositionY);

                }
                else if (MStartPos != null)
                {

                    _pos = new Vector2(MStartPos.MPositionX, MStartPos.MPositionY);

                }
                else
                {

                    _pos = Vector2.zero;

                }
                Instantiate(Resources.Load<Object>("VFXs/hit-white-1"), _pos, Quaternion.identity);
                //音效
                MyCommon.MusicSource[0].Play();
                break;

            case VfxType.Attack2:     //2 攻击2 魔法怪物
                if (MStartPos != null)
                {

                    _pos = new Vector2(MStartPos.MPositionX, MStartPos.MPositionY);

                }
                else if (ActivateAStarPoint != null)
                {

                    _pos = new Vector2(ActivateAStarPoint.MPositionX, ActivateAStarPoint.MPositionY);

                }
                else
                {

                    _pos = Vector2.zero;

                }
                Instantiate(Resources.Load<Object>("VFXs/punch-2"), _pos, Quaternion.identity);
                //音效
                MyCommon.MusicSource[10].Play();
                break;

        }

    }


    /// <summary>
    /// 道具的功能
    /// </summary>
    public void DoPropEvent()
    {

        GameObject _buttonSelf = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
        //Debug.Log(buttonSelf.name);
        for (int _itemIndex = 0; _itemIndex < MyUI.ItemBtns.Count; _itemIndex++)
        {

            if (MyUI.ItemBtns[_itemIndex].gameObject == _buttonSelf)
            {

                StartCoroutine(OpenItemUI(_itemIndex));

            }

        }
        RefreshItemBtn();

    }


    /// <summary>
    /// 打开Item的UI
    /// </summary>
    /// <param name="_index">Index.</param>
    IEnumerator OpenItemUI(int _index)
    {

        //if (ActivateAStarPoint != null)
        //{

        //    yield break;

        //}
        if (!MyPlayer.IsNotMoving())
        {

            yield break;

        }

        string _dbName = Dict.SqlDBName;
        //道具说明 待定 功能是只显示一次 然后关闭
        //string _info = _dict.GetString(_dbName, "prop", "info", _index + 1);
        //DoMessage(_info);
        switch (_index)
        {

            case 0://楼层跳跃器
                yield return new WaitUntil(MyUI.IsAllUIClosed);
                //-UI背景-
                MyUI.BasicUIBox.gameObject.SetActive(true);
                MyUI.BasicUIBox.GetComponent<Image>().raycastTarget = true;
                ////-------
                MyUI.MessagePart.gameObject.SetActive(true);
                MyUI.MessagePart.GetComponentInChildren<Text>().text = "当前楼层 F" + (SaveHeroInfo.CurMapId - 1) + "\n" + "\n" + "您的选择是？";
                MyUI.SelectsPart.gameObject.SetActive(true);
                for (int _i = 0; _i < MyUI.SelectsPart.childCount; _i++)
                {

                    switch (_i)
                    {

                        case 0:
                            MyUI.SelectsPart.GetChild(_i).gameObject.SetActive(true);
                            MyUI.SelectsPart.GetChild(_i).GetComponentInChildren<Text>().text = "↑";
                            MyUI.SelectsPart.GetChild(_i).GetComponent<Button>().onClick.AddListener(MyUI.FloorJumpUp);
                            break;
                        case 1:
                            MyUI.SelectsPart.GetChild(_i).gameObject.SetActive(true);
                            MyUI.SelectsPart.GetChild(_i).GetComponentInChildren<Text>().text = "↓";
                            MyUI.SelectsPart.GetChild(_i).GetComponent<Button>().onClick.AddListener(MyUI.FloorJumpDown);
                            break;
                        case 2:
                            MyUI.SelectsPart.GetChild(_i).gameObject.SetActive(false);
                            break;
                        case 3:
                            MyUI.SelectsPart.GetChild(_i).gameObject.SetActive(false);
                            break;

                    }

                }

                break;

            case 1://火眼金睛
                yield return new WaitUntil(MyUI.IsAllUIClosed);
                //-UI背景-
                MyUI.ItemUIBox.gameObject.SetActive(true);
                GameObject _monsterInfo = (GameObject)Instantiate<Object>(Resources.Load("UI/MonsterInfo")) as GameObject;
                _monsterInfo.transform.SetParent(MyUI.ItemUIBox.GetChild(0));
                _monsterInfo.transform.localScale = Vector3.one;
                _monsterInfo.GetComponent<RectTransform>().anchoredPosition3D = Vector3.zero;
                _monsterInfo.GetComponent<RectTransform>().offsetMin = Vector3.zero;
                _monsterInfo.GetComponent<RectTransform>().offsetMax = Vector3.zero;
                //-------

                //所有MonsterId (包括重复怪物Id)
                List<int> _monsterIds = new List<int>();
                //遍历 map_obj 的 _id
                for (int _id = 1; _id <= Dict.GetColCount(_dbName, "map_obj"); _id++)
                {

                    //MapId 限制为 当前MapId
                    int _mapId = Dict.GetInt(_dbName, "map_obj", "map_id", _id);
                    if (_mapId == SaveHeroInfo.CurMapId)
                    {

                        //TypeId 限制为 14 怪物
                        int _typeId = Dict.GetInt(_dbName, "map_obj", "type_id", _id);
                        if (_typeId == 14 || _typeId == 71)
                        {

                            //Value 限制为 0 Obj当前存在
                            int _value = Dict.GetInt(_dbName, "map_obj", "value", _id);
                            if (_value == 0)
                            {

                                //MiniTypeId 怪物内部的Id
                                int _miniId = Dict.GetInt(_dbName, "map_obj", "mini_type_id", _id);
                                //一个个加入MonsterIds
                                _monsterIds.Add(_miniId);

                            }

                        }

                    }

                }

                //去重
                _monsterIds = _monsterIds.Distinct().ToList();

                for (int _i = 0; _i < _monsterIds.Count; _i++)
                {

                    //MyUI.AddMonsterInfo(MyUI.ItemUIBox, _monsterIds[_i]);

                    int _monHp = Dict.GetInt(_dbName, "monster", "hp", _monsterIds[_i]);
                    int _monAt = Dict.GetInt(_dbName, "monster", "atk", _monsterIds[_i]);
                    int _monDf = Dict.GetInt(_dbName, "monster", "def", _monsterIds[_i]);
                    int _monGold = Dict.GetInt(_dbName, "monster", "gold", _monsterIds[_i]);
                    int _heroAt = SaveHeroInfo.Attributes[2];
                    //装了十字架
                    if (SaveHeroInfo.Props[2] == 0)
                    {

                        if (_monsterIds[_i] == 11 || _monsterIds[_i] == 13 || _monsterIds[_i] == 15)
                        {

                            _heroAt *= 2;

                        }

                    }
                    string _res = Dict.GetString(_dbName, "monster", "res", _monsterIds[_i]);
                    //总伤害值float
                    float _hurtValue = _heroAt <= _monDf ? 0 : _monHp / (_heroAt - _monDf);
                    //实际伤害值int
                    int _changeNum = Dict.IsInteger(_hurtValue.ToString()) ? System.Convert.ToInt32((_hurtValue - 1) * (_monAt - SaveHeroInfo.Attributes[3])) : Mathf.CeilToInt(_hurtValue) * (_monAt - SaveHeroInfo.Attributes[3]);
                    //Hero At <= Monster DF 或 hp <= 实际伤害值
                    if (_heroAt <= _monDf || SaveHeroInfo.Attributes[1] <= _changeNum)
                    {

                        _changeNum = SaveHeroInfo.Attributes[1];

                    }

                    GameObject _singleMonsterInfo = Instantiate<Object>(Resources.Load("UI/SingleMonsterInfo")) as GameObject;
                    _singleMonsterInfo.transform.SetParent(_monsterInfo.transform.GetChild(1));
                    _singleMonsterInfo.transform.localScale = Vector3.one;
                    _singleMonsterInfo.GetComponent<RectTransform>().anchoredPosition3D = Vector3.zero;
                    _singleMonsterInfo.GetComponent<RectTransform>().offsetMin = Vector3.zero;
                    _singleMonsterInfo.GetComponent<RectTransform>().offsetMax = Vector3.zero;
                    _singleMonsterInfo.transform.localPosition = new Vector3(_singleMonsterInfo.transform.localPosition.x, -48 * (_monsterInfo.transform.GetChild(1).childCount - 1));
                    _singleMonsterInfo.transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<GameObject>(_res).GetComponent<SpriteRenderer>().sprite;
                    _singleMonsterInfo.transform.GetChild(1).GetComponent<Text>().text = _monHp.ToString();
                    _singleMonsterInfo.transform.GetChild(2).GetComponent<Text>().text = _monAt.ToString();
                    _singleMonsterInfo.transform.GetChild(3).GetComponent<Text>().text = _monDf.ToString();
                    _singleMonsterInfo.transform.GetChild(4).GetComponent<Text>().text = _monGold.ToString();
                    _singleMonsterInfo.transform.GetChild(5).GetComponent<Text>().text = _changeNum.ToString();

                    /*背景
                    Transform _monsterInfo = CreateImg(_parent, Resources.Load<Sprite>("UI/HeroInfoBG_Col3"), 1f, _height, 0, _y, AnchorPresets.TopLeft, PivotPresets.TopLeft, new Color(0, 0, 0, 0.4f), false, false, true);
                    Transform _monsterImg = CreateImg(_monsterInfo, Resources.Load<GameObject>(_res).GetComponent<SpriteRenderer>().sprite, 1f / 5f, 1f, 0.75f / 6.5f, 0f, AnchorPresets.TopLeft, PivotPresets.TopCenter, Color.white, false, true);
                    CreateText(_monsterInfo, 1f / 5f, 1f, 2f / 6.5f, 0f, AnchorPresets.TopLeft, PivotPresets.TopCenter, TextAnchor.MiddleCenter, Color.white, false, _monHp.ToString(), "血量文字", 28);
                    CreateText(_monsterInfo, 1f / 5f, 1f, 3f / 6.5f, 0f, AnchorPresets.TopLeft, PivotPresets.TopCenter, TextAnchor.MiddleCenter, Color.white, false, _monAt.ToString(), "攻击文字", 28);
                    CreateText(_monsterInfo, 1f / 5f, 1f, 4f / 6.5f, 0f, AnchorPresets.TopLeft, PivotPresets.TopCenter, TextAnchor.MiddleCenter, Color.white, false, _monDf.ToString(), "防御文字", 28);
                    CreateText(_monsterInfo, 1f / 5f, 1f, 5f / 6.5f, 0f, AnchorPresets.TopLeft, PivotPresets.TopCenter, TextAnchor.MiddleCenter, Color.white, false, _monGold.ToString(), "金币文字", 28);
                    CreateText(_monsterInfo, 1f / 5f, 1f, 6f / 6.5f, 0f, AnchorPresets.TopLeft, PivotPresets.TopCenter, TextAnchor.MiddleCenter, Color.white, false, _changeNum.ToString(), "损血文字", 28).GetComponent<Outline>().effectColor = new Color(1f, 0f, 0f, 0.6f);
                    */

                }

                yield return new WaitUntil(MyUI.IsAllUIClosed);
                //强化版火眼金睛
                if (SaveHeroInfo.Attributes[12] == 1)
                {

                    StartCoroutine(DoStory(79));
                    for (int _w = 0; _w < Dict.MapWidth; _w++)
                    {

                        for (int _h = 0; _h < Dict.MapHeight; _h++)
                        {

                            if (MPointGrid[_w, _h].MMapObjId != 0)
                            {

                                if (Dict.GetInt(_dbName, "map_obj", "type_id", MPointGrid[_w, _h].MMapObjId) == 14)
                                {

                                    MPointGrid[_w, _h].UpdateSelf();

                                }

                            }

                        }

                    }

                }

                break;

            case 2://十字架
                break;

            case 3://圣水
                yield return new WaitUntil(MyUI.IsAllUIClosed);
                //-UI背景-
                MyUI.BasicUIBox.gameObject.SetActive(true);
                MyUI.BasicUIBox.GetComponent<Image>().raycastTarget = true;
                ////-------
                MyUI.MessagePart.gameObject.SetActive(true);
                MyUI.MessagePart.GetComponentInChildren<Text>().text = "确认使用？";
                MyUI.SelectsPart.gameObject.SetActive(true);
                for (int _i = 0; _i < MyUI.SelectsPart.childCount; _i++)
                {

                    switch (_i)
                    {

                        case 0:
                            MyUI.SelectsPart.GetChild(_i).gameObject.SetActive(true);
                            MyUI.SelectsPart.GetChild(_i).GetComponentInChildren<Text>().text = "是的";
                            MyUI.SelectsPart.GetChild(_i).GetComponent<Button>().onClick.AddListener(MyUI.HolyWater);
                            break;
                        case 1:
                            MyUI.SelectsPart.GetChild(_i).gameObject.SetActive(true);
                            MyUI.SelectsPart.GetChild(_i).GetComponentInChildren<Text>().text = "还没想好";
                            MyUI.SelectsPart.GetChild(_i).GetComponent<Button>().onClick.AddListener(CloseUIBox);
                            break;
                        case 2:
                            MyUI.SelectsPart.GetChild(_i).gameObject.SetActive(false);
                            break;
                        case 3:
                            MyUI.SelectsPart.GetChild(_i).gameObject.SetActive(false);
                            break;

                    }

                }
                break;

            case 4://黄金钥匙
                yield return new WaitUntil(MyUI.IsAllUIClosed);
                //-UI背景-
                MyUI.BasicUIBox.gameObject.SetActive(true);
                MyUI.BasicUIBox.GetComponent<Image>().raycastTarget = true;
                ////-------
                MyUI.MessagePart.gameObject.SetActive(true);
                MyUI.MessagePart.GetComponentInChildren<Text>().text = "确认使用？";
                MyUI.SelectsPart.gameObject.SetActive(true);
                for (int _i = 0; _i < MyUI.SelectsPart.childCount; _i++)
                {

                    switch (_i)
                    {

                        case 0:
                            MyUI.SelectsPart.GetChild(_i).gameObject.SetActive(true);
                            MyUI.SelectsPart.GetChild(_i).GetComponentInChildren<Text>().text = "是的";
                            MyUI.SelectsPart.GetChild(_i).GetComponent<Button>().onClick.AddListener(MyUI.GoldKey);
                            break;
                        case 1:
                            MyUI.SelectsPart.GetChild(_i).gameObject.SetActive(true);
                            MyUI.SelectsPart.GetChild(_i).GetComponentInChildren<Text>().text = "还没想好";
                            MyUI.SelectsPart.GetChild(_i).GetComponent<Button>().onClick.AddListener(CloseUIBox);
                            break;
                        case 2:
                            MyUI.SelectsPart.GetChild(_i).gameObject.SetActive(false);
                            break;
                        case 3:
                            MyUI.SelectsPart.GetChild(_i).gameObject.SetActive(false);
                            break;

                    }

                }
                break;

            case 5://向下飞行器
                yield return new WaitUntil(MyUI.IsAllUIClosed);
                //-UI背景-
                MyUI.BasicUIBox.gameObject.SetActive(true);
                MyUI.BasicUIBox.GetComponent<Image>().raycastTarget = true;
                ////-------
                MyUI.MessagePart.gameObject.SetActive(true);
                MyUI.MessagePart.GetComponentInChildren<Text>().text = "确认使用？";
                MyUI.SelectsPart.gameObject.SetActive(true);
                for (int _i = 0; _i < MyUI.SelectsPart.childCount; _i++)
                {

                    switch (_i)
                    {

                        case 0:
                            MyUI.SelectsPart.GetChild(_i).gameObject.SetActive(true);
                            MyUI.SelectsPart.GetChild(_i).GetComponentInChildren<Text>().text = "是的";
                            MyUI.SelectsPart.GetChild(_i).GetComponent<Button>().onClick.AddListener(MyUI.FloorDown);
                            break;
                        case 1:
                            MyUI.SelectsPart.GetChild(_i).gameObject.SetActive(true);
                            MyUI.SelectsPart.GetChild(_i).GetComponentInChildren<Text>().text = "还没想好";
                            MyUI.SelectsPart.GetChild(_i).GetComponent<Button>().onClick.AddListener(CloseUIBox);
                            break;
                        case 2:
                            MyUI.SelectsPart.GetChild(_i).gameObject.SetActive(false);
                            break;
                        case 3:
                            MyUI.SelectsPart.GetChild(_i).gameObject.SetActive(false);
                            break;

                    }

                }
                break;

            case 6://向上飞行器
                yield return new WaitUntil(MyUI.IsAllUIClosed);
                //-UI背景-
                MyUI.BasicUIBox.gameObject.SetActive(true);
                MyUI.BasicUIBox.GetComponent<Image>().raycastTarget = true;
                ////-------
                MyUI.MessagePart.gameObject.SetActive(true);
                MyUI.MessagePart.GetComponentInChildren<Text>().text = "确认使用？";
                MyUI.SelectsPart.gameObject.SetActive(true);
                for (int _i = 0; _i < MyUI.SelectsPart.childCount; _i++)
                {

                    switch (_i)
                    {

                        case 0:
                            MyUI.SelectsPart.GetChild(_i).gameObject.SetActive(true);
                            MyUI.SelectsPart.GetChild(_i).GetComponentInChildren<Text>().text = "是的";
                            MyUI.SelectsPart.GetChild(_i).GetComponent<Button>().onClick.AddListener(MyUI.FloorUp);
                            break;
                        case 1:
                            MyUI.SelectsPart.GetChild(_i).gameObject.SetActive(true);
                            MyUI.SelectsPart.GetChild(_i).GetComponentInChildren<Text>().text = "还没想好";
                            MyUI.SelectsPart.GetChild(_i).GetComponent<Button>().onClick.AddListener(CloseUIBox);
                            break;
                        case 2:
                            MyUI.SelectsPart.GetChild(_i).gameObject.SetActive(false);
                            break;
                        case 3:
                            MyUI.SelectsPart.GetChild(_i).gameObject.SetActive(false);
                            break;

                    }

                }
                break;

            case 7://幸运金币
                break;

            case 8://炸药
                yield return new WaitUntil(MyUI.IsAllUIClosed);
                //-UI背景-
                MyUI.BasicUIBox.gameObject.SetActive(true);
                MyUI.BasicUIBox.GetComponent<Image>().raycastTarget = true;
                ////-------
                MyUI.MessagePart.gameObject.SetActive(true);
                MyUI.MessagePart.GetComponentInChildren<Text>().text = "确认使用？";
                MyUI.SelectsPart.gameObject.SetActive(true);
                for (int _i = 0; _i < MyUI.SelectsPart.childCount; _i++)
                {

                    switch (_i)
                    {

                        case 0:
                            MyUI.SelectsPart.GetChild(_i).gameObject.SetActive(true);
                            MyUI.SelectsPart.GetChild(_i).GetComponentInChildren<Text>().text = "是的";
                            MyUI.SelectsPart.GetChild(_i).GetComponent<Button>().onClick.AddListener(MyUI.Explosive);
                            break;
                        case 1:
                            MyUI.SelectsPart.GetChild(_i).gameObject.SetActive(true);
                            MyUI.SelectsPart.GetChild(_i).GetComponentInChildren<Text>().text = "还没想好";
                            MyUI.SelectsPart.GetChild(_i).GetComponent<Button>().onClick.AddListener(CloseUIBox);
                            break;
                        case 2:
                            MyUI.SelectsPart.GetChild(_i).gameObject.SetActive(false);
                            break;
                        case 3:
                            MyUI.SelectsPart.GetChild(_i).gameObject.SetActive(false);
                            break;

                    }

                }
                break;

            case 9://屠龙匕
                yield return new WaitUntil(MyUI.IsAllUIClosed);
                break;

            case 10://避水珠
                yield return new WaitUntil(MyUI.IsAllUIClosed);
                break;

            case 11://芭蕉扇
                yield return new WaitUntil(MyUI.IsAllUIClosed);
                //-UI背景-
                MyUI.BasicUIBox.gameObject.SetActive(true);
                MyUI.BasicUIBox.GetComponent<Image>().raycastTarget = true;
                ////-------
                MyUI.MessagePart.gameObject.SetActive(true);
                MyUI.MessagePart.GetComponentInChildren<Text>().text = "确认使用？";
                MyUI.SelectsPart.gameObject.SetActive(true);
                for (int _i = 0; _i < MyUI.SelectsPart.childCount; _i++)
                {

                    switch (_i)
                    {

                        case 0:
                            MyUI.SelectsPart.GetChild(_i).gameObject.SetActive(true);
                            MyUI.SelectsPart.GetChild(_i).GetComponentInChildren<Text>().text = "是的";
                            MyUI.SelectsPart.GetChild(_i).GetComponent<Button>().onClick.AddListener(MyUI.BaJiaoShan);
                            break;
                        case 1:
                            MyUI.SelectsPart.GetChild(_i).gameObject.SetActive(true);
                            MyUI.SelectsPart.GetChild(_i).GetComponentInChildren<Text>().text = "还没想好";
                            MyUI.SelectsPart.GetChild(_i).GetComponent<Button>().onClick.AddListener(CloseUIBox);
                            break;
                        case 2:
                            MyUI.SelectsPart.GetChild(_i).gameObject.SetActive(false);
                            break;
                        case 3:
                            MyUI.SelectsPart.GetChild(_i).gameObject.SetActive(false);
                            break;

                    }

                }
                break;

            case 12://猴毛
                yield return new WaitUntil(MyUI.IsAllUIClosed);
                //-UI背景-
                MyUI.BasicUIBox.gameObject.SetActive(true);
                MyUI.BasicUIBox.GetComponent<Image>().raycastTarget = true;
                ////-------
                MyUI.MessagePart.gameObject.SetActive(true);
                MyUI.MessagePart.GetComponentInChildren<Text>().text = "确认使用？";
                MyUI.SelectsPart.gameObject.SetActive(true);
                for (int _i = 0; _i < MyUI.SelectsPart.childCount; _i++)
                {

                    switch (_i)
                    {

                        case 0:
                            MyUI.SelectsPart.GetChild(_i).gameObject.SetActive(true);
                            MyUI.SelectsPart.GetChild(_i).GetComponentInChildren<Text>().text = "是的";
                            MyUI.SelectsPart.GetChild(_i).GetComponent<Button>().onClick.AddListener(MyUI.HouMao);
                            break;
                        case 1:
                            MyUI.SelectsPart.GetChild(_i).gameObject.SetActive(true);
                            MyUI.SelectsPart.GetChild(_i).GetComponentInChildren<Text>().text = "还没想好";
                            MyUI.SelectsPart.GetChild(_i).GetComponent<Button>().onClick.AddListener(CloseUIBox);
                            break;
                        case 2:
                            MyUI.SelectsPart.GetChild(_i).gameObject.SetActive(false);
                            break;
                        case 3:
                            MyUI.SelectsPart.GetChild(_i).gameObject.SetActive(false);
                            break;

                    }

                }
                break;

            case 13://铁镐
                yield return new WaitUntil(MyUI.IsAllUIClosed);
                //-UI背景-
                MyUI.BasicUIBox.gameObject.SetActive(true);
                MyUI.BasicUIBox.GetComponent<Image>().raycastTarget = true;
                ////-------
                MyUI.MessagePart.gameObject.SetActive(true);
                MyUI.MessagePart.GetComponentInChildren<Text>().text = "确认使用？";
                MyUI.SelectsPart.gameObject.SetActive(true);
                for (int _i = 0; _i < MyUI.SelectsPart.childCount; _i++)
                {

                    switch (_i)
                    {

                        case 0:
                            MyUI.SelectsPart.GetChild(_i).gameObject.SetActive(true);
                            MyUI.SelectsPart.GetChild(_i).GetComponentInChildren<Text>().text = "是的";
                            MyUI.SelectsPart.GetChild(_i).GetComponent<Button>().onClick.AddListener(MyUI.Mattock);
                            break;
                        case 1:
                            MyUI.SelectsPart.GetChild(_i).gameObject.SetActive(true);
                            MyUI.SelectsPart.GetChild(_i).GetComponentInChildren<Text>().text = "还没想好";
                            MyUI.SelectsPart.GetChild(_i).GetComponent<Button>().onClick.AddListener(CloseUIBox);
                            break;
                        case 2:
                            MyUI.SelectsPart.GetChild(_i).gameObject.SetActive(false);
                            break;
                        case 3:
                            MyUI.SelectsPart.GetChild(_i).gameObject.SetActive(false);
                            break;

                    }

                }
                break;

            case 14://地震卷轴
                yield return new WaitUntil(MyUI.IsAllUIClosed);
                //-UI背景-
                MyUI.BasicUIBox.gameObject.SetActive(true);
                MyUI.BasicUIBox.GetComponent<Image>().raycastTarget = true;
                ////-------
                MyUI.MessagePart.gameObject.SetActive(true);
                MyUI.MessagePart.GetComponentInChildren<Text>().text = "确认使用？";
                MyUI.SelectsPart.gameObject.SetActive(true);
                for (int _i = 0; _i < MyUI.SelectsPart.childCount; _i++)
                {

                    switch (_i)
                    {

                        case 0:
                            MyUI.SelectsPart.GetChild(_i).gameObject.SetActive(true);
                            MyUI.SelectsPart.GetChild(_i).GetComponentInChildren<Text>().text = "是的";
                            MyUI.SelectsPart.GetChild(_i).GetComponent<Button>().onClick.AddListener(MyUI.ShakeScroll);
                            break;
                        case 1:
                            MyUI.SelectsPart.GetChild(_i).gameObject.SetActive(true);
                            MyUI.SelectsPart.GetChild(_i).GetComponentInChildren<Text>().text = "还没想好";
                            MyUI.SelectsPart.GetChild(_i).GetComponent<Button>().onClick.AddListener(CloseUIBox);
                            break;
                        case 2:
                            MyUI.SelectsPart.GetChild(_i).gameObject.SetActive(false);
                            break;
                        case 3:
                            MyUI.SelectsPart.GetChild(_i).gameObject.SetActive(false);
                            break;

                    }

                }
                break;

            case 15://筋斗云
                yield return new WaitUntil(MyUI.IsAllUIClosed);
                int _x = 10 - MStartPos.MPositionX;
                int _y = 10 - MStartPos.MPositionY;
                //如果翻转后的位置不为obj
                if (!MPointGrid[_x, _y].IsObj)
                {

                    MStartPos = MPointGrid[_x, _y];
                    MyPlayer.MGameObject.transform.position = new Vector3(_x, _y);

                }
                else
                {

                    int[] _canGetItems = {

                    4,5,6,9,10,11,12,15,19,20,21,22,23,24,25,26,27,28,29,30,31,32,33,34,35,36,37,38,39,40,41,43,44,45,46,47,48,49,50,51,52,53,54,55,56,57,58,59,60,61,62,63,64,66,68

                    };

                    foreach (int _e in _canGetItems)
                    {

                        if (MPointGrid[_x, _y].MMapObjId <= 0)
                        {

                            yield break;

                        }

                        if (_e == Dict.GetInt(_dbName, "map_obj", "type_id", MPointGrid[_x, _y].MMapObjId))
                        {

                            if (SaveHeroInfo.Attributes[13] == 1)
                            {

                                TriggerObj(MPointGrid[_x, _y].MMapObjId);
                                if (!IsTriggering)
                                {

                                    MStartPos = MPointGrid[_x, _y];
                                    MStartPos.MParentPoint = null;
                                    MyPlayer.MGameObject.transform.position = new Vector3(_x, _y);
                                    SaveHeroInfo.HeroTilePosX = MStartPos.MPositionX;
                                    SaveHeroInfo.HeroTilePosY = MStartPos.MPositionY;
                                    break;

                                }

                            }

                        }

                    }

                }
                break;

        }

    }


    /// <summary>
    /// 执行story事件
    /// </summary>
    /// <param name="_storyid">Storyid.</param>
    IEnumerator DoStoryEvent(int _storyid)
    {

        string _dbName = Dict.SqlDBName;
        string _words = "";
        int _selectId = 0;
        int _changeId = 0;
        //1、主菜单      2、地图       3、点击切屏
        int _nextSceneId = 0;
        //id为1         id为地图id    id为touch_trans_scene表中的id
        int _nextSceneParam = 0;
        int _mapObjChangeId = 0;
        Button boxCloseButton = MyUI.BasicUIBox.gameObject.GetComponent<Button>();
        MyUI.BasicUIBox.GetComponent<Image>().raycastTarget = true;
        _words = Dict.GetString(_dbName, "story", "words", _storyid);
        DoMessage(_words);
        _selectId = Dict.GetInt(_dbName, "story", "select_id", _storyid);
        if (_selectId > 0)
        {

            yield return new WaitUntil(MyUI.IsAllUIClosed);
            MyUI.BasicUIBox.GetComponent<Image>().raycastTarget = true;
            DoHeroSelect(_selectId);

        }
        else
        {

            MyUI.SelectsPart.gameObject.SetActive(false);

        }
        _changeId = Dict.GetInt(_dbName, "story", "change_id", _storyid);
        if (_changeId > 0)
        {

            yield return new WaitUntil(MyUI.IsAllUIClosed);
            MyUI.BasicUIBox.GetComponent<Image>().raycastTarget = true;
            DoPropertyChange(_changeId);

        }
        _nextSceneId = Dict.GetInt(_dbName, "story", "next_scene_id", _storyid);
        if (_nextSceneId > 0)
        {

            _nextSceneParam = Dict.GetInt(_dbName, "story", "next_scene_param", _storyid);
            switch (_nextSceneId)
            {

                case 1://主菜单
                    Debug.Log("返回主菜单");
                    break;

                case 2://地图id
                    Debug.Log("地图id");
                    break;

                case 3://点击切屏 touch_trans_scene表中的id
                    Debug.Log("切到touch_trans_scene表");
                    break;

            }

        }
        _mapObjChangeId = Dict.GetInt(_dbName, "story", "map_obj_change_id", _storyid);

        //特殊情况 60层 MapId61 StoryId32 观音 设为消失
        if (_storyid == 32)
        {

            Dict.SetInt(_dbName, "map_obj", "value", 1482, 1);
            MPointGrid[Dict.GetInt(_dbName, "map_obj", "pos_x", 1482), Dict.GetInt(_dbName, "map_obj", "pos_y", 1482)].UpdateSelf();

        }
        //----------------------------

    }


    /// <summary>
    /// UI消息、选择Box打开
    /// </summary>
    /// <param name="_storyid">Storyid.</param>
    IEnumerator DoStory(int _storyid)
    {

        yield return new WaitUntil(MyPlayer.IsNotMoving);
        yield return new WaitUntil(MyUI.IsAllUIClosed);

        string _dbName = Dict.SqlDBName;

        //-----------------------------------------循环过程--------------------------------------------

        int _nextStory = 0;
        StartCoroutine(DoStoryEvent(_storyid));
        _nextStory = Dict.GetInt(_dbName, "story", "next_story_id", _storyid);
        while (_nextStory > 0)
        {

            _storyid = _nextStory;
            StartCoroutine(DoStoryEvent(_storyid));
            _nextStory = Dict.GetInt(_dbName, "story", "next_story_id", _storyid);
            yield return new WaitUntil(MyUI.IsAllUIClosed);

        }

        //-------------------------------------------------------------------------------------
        //功能未实现 本游戏中不使用
        //int nextStoryId;
        yield return new WaitForSeconds(0.1f);
        //Story的value 0 未使用  1 已使用
        Dict.SetInt(_dbName, "story", "value", _storyid, 1);
        DoSwitchOffByStoryId(_storyid);
        DoSwitchOnByStoryId(_storyid);
        IsAutoMoving = false;

    }


    /// <summary>
    /// 打开选择盒子
    /// </summary>
    void DoHeroSelect(int _selectid)
    {

        string _dbName = Dict.SqlDBName;
        MyUI.BasicUIBox.gameObject.SetActive(true);
        MyUI.SelectsPart.gameObject.SetActive(true);
        string _words = "";
        int _selectNum;
        //是升级雕像
        if (MyUI.IsShop(_selectid))
        {

            _words = Dict.GetString(_dbName, "hero_select", "words", _selectid);
            DoMessage(_words);
            _selectNum = Dict.GetInt(_dbName, "hero_select", "select_num", _selectid);
            for (int i = 0; i < MyUI.SelectsPart.childCount; i++)
            {

                _words = MyUI.MessagePart.GetChild(0).GetComponent<Text>().text.Replace("[20]", "" + SaveHeroInfo.ShopPrice + "");
                DoMessage(_words);
                if (i <= _selectNum - 1)
                {

                    Button selectButton = MyUI.SelectsPart.GetChild(i).GetComponent<Button>();
                    selectButton.gameObject.SetActive(true);
                    selectButton.onClick.RemoveAllListeners();
                    //绑定功能
                    selectButton.onClick.AddListener(() => DoSelectEvent(_selectid));
                    //按钮上文字
                    MyUI.SelectsPart.GetChild(i).GetChild(0).GetComponent<Text>().text = Dict.GetString(_dbName, "hero_select", "select_0" + (i + 1), _selectid);

                }

            }

        }
        //非升级雕像
        else
        {

            _words = Dict.GetString(_dbName, "hero_select", "words", _selectid);
            DoMessage(_words);
            _selectNum = Dict.GetInt(_dbName, "hero_select", "select_num", _selectid);
            for (int i = 0; i < MyUI.SelectsPart.childCount; i++)
            {

                if (i <= _selectNum - 1)
                {

                    Button selectButton = MyUI.SelectsPart.GetChild(i).GetComponent<Button>();
                    selectButton.gameObject.SetActive(true);
                    selectButton.onClick.RemoveAllListeners();
                    //绑定功能
                    selectButton.onClick.AddListener(() => DoSelectEvent(_selectid));
                    //按钮上文字
                    MyUI.SelectsPart.GetChild(i).GetChild(0).GetComponent<Text>().text = Dict.GetString(_dbName, "hero_select", "select_0" + (i + 1), _selectid);

                }

            }

        }

    }


    /// <summary>
    /// 打开消息盒子
    /// </summary>
    /// <param name="_message">Message.</param>
    void DoMessage(string _message)
    {

        string _dbName = Dict.SqlDBName;
        MyUI.BasicUIBox.gameObject.SetActive(true);

        //是选项
        Text messageText = MyUI.MessagePart.GetChild(0).GetComponent<Text>();
        //是消息
        MyUI.MessagePart.gameObject.SetActive(true);
        for (int i = 0; i < MyUI.MessagePart.childCount; i++)
        {

            MyUI.MessagePart.GetChild(i).gameObject.SetActive(true);

        }
        messageText.text = _message;

    }


    /// <summary>
    /// Select按钮的功能(0 - 3)
    /// </summary>
    /// <param name="_selectid">Selectid.(0 - 3)</param>
    void DoSelectEvent(int _selectid)
    {

        GameObject _buttonSelf = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
        //Debug.Log(buttonSelf.name);
        string _dbName = Dict.SqlDBName;
        string _change_id_id = "change_id_0";
        for (int i = 0; i < MyUI.SelectsPart.childCount; i++)
        {

            if (MyUI.SelectsPart.GetChild(i).gameObject == _buttonSelf)
            {

                _change_id_id += i + 1;

            }

        }

        int _changeId = Dict.GetInt(_dbName, "hero_select", _change_id_id, _selectid);

        //int _changeType = _dict.GetInt(_dbName, "property_change", "change_type", _changeId);
        //if (_changeType <= 0)
        //{

        //    //取消的情况
        //    CloseUIBox();
        //    return;

        //}

        //属性改变成功
        if (DoPropertyChange(_changeId))
        {

            //属性改变true
            CloseUIBox();
            //是升级商店
            if (MyUI.IsShop(_selectid))
            {

                SaveHeroInfo.PurchaseCount++;
                return;

            }

            //特殊情况
            switch (_selectid)
            {

                case 7://嫁衣神功
                    if (_change_id_id == "change_id_01")
                    {

                        SaveHeroInfo.PurchaseCount = 0;

                    }
                    break;
                case 8://筋斗云
                    SaveHeroInfo.Attributes[13] = _change_id_id == "change_id_01" ? 1 : 0;
                    break;
                case 9://提升火眼金睛
                    SaveHeroInfo.Attributes[12] = _change_id_id == "change_id_01" ? 1 : 0;
                    break;


            }

        }

        ////【待定】
        //if (ActivateAStarPoint.MMapObjId != 0)
        //{

        //    UpdateSwitch(ActivateAStarPoint.MMapObjId);

        //}



    }


    /// <summary>
    /// 关闭UIBox
    /// </summary>
    public void CloseUIBox()
    {

        MyUI.BasicUIBox.GetComponent<Image>().raycastTarget = true;
        MyUI.ItemUIBox.GetComponent<Image>().raycastTarget = true;
        MyUI.MessagePart.gameObject.SetActive(false);
        for (int _index = 0; _index < MyUI.SelectsPart.childCount; _index++)
        {

            MyUI.SelectsPart.GetChild(_index).GetComponent<Button>().onClick.RemoveAllListeners();
            MyUI.SelectsPart.GetChild(_index).gameObject.SetActive(false);

        }

        for (int _index = 0; _index < MyUI.ItemUIBox.GetChild(0).childCount; _index++)
        {

            Destroy(MyUI.ItemUIBox.GetChild(0).GetChild(_index).gameObject);

        }

        for (int _index = 0; _index < MyUI.RootUI.childCount; _index++)
        {

            MyUI.RootUI.GetChild(_index).gameObject.SetActive(false);

        }
        Time.timeScale = 1;

        /*
        //清空MonsterInfo
        if (MyUI.RootUI.GetChild(1).childCount > 1)
        {

            //除_i = 0 的 Title部分, 其他都移除
            for (int _i = 1; _i < MyUI.RootUI.GetChild(1).childCount; _i++)
            {

                Destroy(MyUI.RootUI.GetChild(1).GetChild(_i).gameObject);

            }

        }

        //隐藏RootUI 二级物件UI 及 三级物件UI
        for (int _i = 0; _i < MyUI.RootUI.childCount; _i++)
        {

            for (int j = 0; j < MyUI.RootUI.GetChild(_i).childCount; j++)
            {

                MyUI.RootUI.GetChild(_i).GetChild(j).gameObject.SetActive(false);
                if (MyUI.RootUI.GetChild(_i).GetChild(j) == MyUI.MessagePart || MyUI.RootUI.GetChild(_i).GetChild(j) == MyUI.SelectsPart)
                {

                    for (int e = 0; e < MyUI.RootUI.GetChild(_i).GetChild(j).childCount; e++)
                    {

                        MyUI.RootUI.GetChild(_i).GetChild(j).GetChild(e).gameObject.SetActive(false);

                    }

                }

            }
            MyUI.RootUI.GetChild(_i).gameObject.SetActive(false);

        }

        //消除激活的MapObj
        //DestroyActiveMapObj();
        ////是升级雕像部分
        //if (minitypeid <= 4 || minitypeid == 27 || minitypeid == 29) return;
        //if (tarpoint == null) return;
        ////0存在 1消失
        //_dict.SetInt(_dict.SqlDBName, "map_obj", "value", tarpoint.MMapObjId, 1);
        ////刷新点状态
        //tarpoint.UpdateSelf(tarpoint.MMapObjId);
        */

    }


    /// <summary>
    /// 消除MapObj
    /// </summary>
    public void DestroyMapObj(AStarPoint _tarpoint)
    {

        if (_tarpoint == null) return;
        //0存在 1消失
        Dict.SetInt(Dict.SqlDBName, "map_obj", "value", _tarpoint.MMapObjId, 1);
        //刷新点状态
        _tarpoint.UpdateSelf();

        if (!MyUI.IsAllUIClosed())
        {

            return;

        }
        ////保存Map Obj Id数据
        //SaveMapObjById(_tarpoint.MMapObjId);

    }


    ///// <summary>
    ///// 楼层跳跃器功能
    ///// </summary>
    //public void JumpToFloor()
    //{

    //    GameObject buttonSelf = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
    //    //Debug.Log(buttonSelf.name);

    //    int _btnMapId = 1;
    //    //遍历所有Item0 楼层跳跃器UI中的按钮
    //    for (int i = 0; i < MyUI.RootUI.GetChild(0).childCount; i++)
    //    {

    //        if (MyUI.RootUI.GetChild(0).GetChild(i).gameObject == buttonSelf)
    //        {

    //            _btnMapId = i + 1;
    //            Debug.Log("地图MapId: " + _btnMapId);

    //        }

    //    }

    //    EnterMapId(_btnMapId);

    //}


    /// <summary>
    /// 更新ItemBtn
    /// </summary>
    public void RefreshItemBtn()
    {

        for (int i = 0; i < MyUI.ItemBtns.Count; i++)
        {

            MyUI.ItemBtns[i].GetComponent<Image>().raycastTarget = MyUI.ItemBtns[i].GetComponent<Image>().sprite == MyUI.ItemUIImages[i];

        }

    }


    /// <summary>
    /// 获得指定PropertyChangeId的全部关联Id
    /// </summary>
    /// <returns>The all identifiers.</returns>
    /// <param name="_id">PropertyChangeId.</param>
    List<int> GetAllLinkPropertyChangeIds(int _id)
    {

        List<int> _ids = new List<int>
        {

            _id

        };

        while (Dict.GetInt(Dict.SqlDBName, "property_change", "next_change_id", _id) > 0)
        {

            _id = Dict.GetInt(Dict.SqlDBName, "property_change", "next_change_id", _id);
            _ids.Add(_id);

        }

        return _ids;

    }

}