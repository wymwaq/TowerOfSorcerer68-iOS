using UnityEngine;

public enum PointType
{

    Through = 0,
    Obstacle = 1,
    TempThrough = 2

}


/// <summary>
/// 存储寻路点信息
/// </summary>
public class AStarPoint
{

    //父“格子”
    public AStarPoint MParentPoint { get; set; }
    //格子显示对象
    public GameObject MGameObject { get; set; }


    public float MF { get; set; }
    public float MG { get; set; }
    public float MH { get; set; }
    //点的位置
    public Vector2 MPosition { get; set; }
    public int MPositionX { get; set; }
    public int MPositionY { get; set; }
    //该点是否处于障碍物
    //public bool MIsObstacle { get; set; }
    public PointType MType { get; set; }
    public bool IsObj { get; set; }
    public bool IsWall { get; set; }


    /// <summary>
    /// Dict中map_obj表中的id 总1900
    /// </summary>
    /// <value>The map object identifier.</value>
    public int MMapObjId { get; set; }


    public int MWallId { get; set; }


    /// <summary>
    /// 为warp_point时用 WarpPoint中的id
    /// </summary>
    /// <value>The tar map identifier.</value>
    public int MWarpPointId { get; set; }


    /// <summary>
    /// Auto_Move
    /// </summary>
    /// <value>The MA uto move identifier.</value>
    public int MAutoMoveId { get; set; }


    public System.Collections.Generic.List<AStarPoint> MConnectPoint;


    //public int MMapId { get; set; }
    //public int MTypeId { get; set; }
    //public int MMiniTypeId { get; set; }
    public AStarPoint(int positionX, int positionY)
    {

        this.MPositionX = positionX;
        this.MPositionY = positionY;
        this.MPosition = new Vector2(MPositionX, MPositionY);
        this.MParentPoint = null;

    }

    public bool ObjIsNull()
    {

        return MGameObject == null;

    }


    /// <summary>
    /// 图块更新自己
    /// </summary>
    public void UpdateSelf()
    {


        GameManager _gm = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();

        //if (MMapObjId == 0)
        //{

        //    return;

        //}
        string _dbName = "";
        string _tabName = "";
        int _id = 0;
        if (IsWall)
        {

            _dbName = Dict.WallDBName;
            _tabName = "map_" + SaveHeroInfo.CurMapId;
            _id = MWallId;

        }
        else
        {

            _dbName = Dict.SqlDBName;
            _tabName = "map_obj";
            _id = MMapObjId;

        }

        //value 0 出现
        if (Dict.GetInt(_dbName, _tabName, "value", _id) == 0)
        {

            //创造
            _gm.GetMap().CreateMapObjById(_id);

        }
        //value 1 消失
        else
        {

            AStarPoint _selfPoint = _gm.MPointGrid[MPositionX, MPositionY];
            Object.Destroy(_selfPoint.MGameObject);
            _selfPoint.MGameObject = null;
            if (_gm.MPointGrid[MPositionX, MPositionY].MConnectPoint == null)
            {

                _gm.MPointGrid[MPositionX, MPositionY].Clear();

            }
            else
            {

                //针对体型较大如巨龙或章鱼
                foreach (AStarPoint _p in _gm.MPointGrid[MPositionX, MPositionY].MConnectPoint)
                {

                    _gm.MPointGrid[_p.MPositionX, _p.MPositionY].Clear();

                }

            }

        }

        if (MType == PointType.TempThrough)
        {

            MType = PointType.Obstacle;

        }

    }


    public void Clear()
    {

        //暂定是否需要清除
        MF = 0;
        MG = 0;
        MH = 0;
        //MMapId = 0;
        MMapObjId = 0;
        MWallId = 0;
        MWarpPointId = 0;
        //MMiniTypeId = 0;
        //MTypeId = 0;

        MParentPoint = null;
        MAutoMoveId = 0;
        IsObj = false;
        IsWall = false;

        Object.Destroy(MGameObject);
        MGameObject = null;
        MConnectPoint = null;
        MType = PointType.Through;

    }

}
