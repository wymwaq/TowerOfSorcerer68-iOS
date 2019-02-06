using UnityEngine;

//public enum CubeType
//{

//    BaseGround, Obj, WarpPoint

//}

public class Cube : MonoBehaviour
{

    public delegate void VoidDelegate(int x, int y);
    public VoidDelegate FindPath;


    public void OnMouseDown()
    {

        GameManager _gm = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        _gm.MPathPosList = null;
        if (_gm.IsTriggering || _gm.IsAutoMoving || !_gm.MyUI.IsAllUIClosed())
        {

            return;

        }

        for (int x = 0; x < Dict.MapWidth; x++)
        {

            for (int y = 0; y < Dict.MapHeight; y++)
            {

                //将临时可通行的格子重置为不可通行
                if (_gm.MPointGrid[x, y].MType == PointType.TempThrough)
                {

                    _gm.MPointGrid[x, y].MType = PointType.Obstacle;

                }

            }

        }

        int _posX = System.Convert.ToInt32(transform.position.x);
        int _posY = System.Convert.ToInt32(transform.position.y);

        if (FindPath != null)
        {

            AStarPoint _astarPoint = _gm.MPointGrid[_posX, _posY];
            _gm.ActivateAStarPoint = _astarPoint;
            //如果是Obj/Item物件 图块置为可通过
            if (_astarPoint.IsObj)
            {

                _astarPoint.MType = PointType.TempThrough;

            }

            FindPath(_posX, _posY);

        }

        //23层 MapId24 佛家符号的中心位置处图块
        if (SaveHeroInfo.CurMapId == 24)
        {

            if (_posX == 5 && _posY == 5)
            {

                if (_gm.MStartPos.MPositionX == 4 && _gm.MStartPos.MPositionY == 6 ||
                    _gm.MStartPos.MPositionX == 6 && _gm.MStartPos.MPositionY == 6 ||
                    _gm.MStartPos.MPositionX == 4 && _gm.MStartPos.MPositionY == 4 ||
                    _gm.MStartPos.MPositionX == 6 && _gm.MStartPos.MPositionY == 4
                    )
                {

                    //中心处图块的MapObjId = 811  
                    _gm.TriggerObj(811);

                }

            }

        }

    }

}