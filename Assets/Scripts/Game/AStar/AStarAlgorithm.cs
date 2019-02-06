using System.Collections.Generic;
using UnityEngine;

public class AStarAlgorithm
{

    public static bool Dir8 { get; set; }

    public static int MGridWidth { get; set; }

    public static int MGridHeight { get; set; }

    public static Transform PathRoot { get; set; }

    //public static Shader PathShader = Shader.Find("PathLight");
    static Color PathColor = Color.yellow;

    //使用二维数组存储点网格    
    public AStarPoint[,] mPointGrid = new AStarPoint[MGridWidth, MGridHeight];

    //存储路径方格子
    public List<AStarPoint> mPathPosList = new List<AStarPoint>();

    static AStarAlgorithm _instance;
    public static AStarAlgorithm GetInsatnce
    {

        get
        {

            if (_instance == null)
            {

                _instance = new AStarAlgorithm();

            }

            return _instance;

        }

    }


    public AStarAlgorithm()
    {

        InitPoint();

    }

    //在网格上设置点的信息
    void InitPoint()
    {

        for (int i = 0; i < MGridWidth; i++)
        {

            for (int j = 0; j < MGridHeight; j++)
            {

                mPointGrid[i, j] = new AStarPoint(i, j);

            }

        }

        //显示障碍物
        for (int x = 0; x < MGridWidth; x++)
        {

            for (int y = 0; y < MGridHeight; y++)
            {

                if (mPointGrid[x, y].MType == PointType.Obstacle)
                {

                    //Blue
                    CreatePath(x, y, Color.blue);

                }

            }

        }

    }


    public void ClearGrid()
    {

        for (int x = 0; x < MGridWidth; x++)
        {

            for (int y = 0; y < MGridHeight; y++)
            {

                if (mPointGrid[x, y].MType == PointType.Through)
                {

                    if (mPointGrid[x, y].MGameObject != null)
                    {

                        Object.Destroy(mPointGrid[x, y].MGameObject);
                        mPointGrid[x, y].MGameObject = null;

                        //重新设置父节点
                        mPointGrid[x, y].MParentPoint = null;

                    }

                }
                else if (mPointGrid[x, y].MType == PointType.TempThrough)
                {

                    //如果全都不清空 会进入死循环
                    if (mPointGrid[x, y].MGameObject != null)
                    {

                        //Debug.Log(mPointGrid[x, y].MGameObject.name);
                        //重新设置父节点 
                        mPointGrid[x, y].MParentPoint = null;

                    }

                }

            }

        }

    }


    //寻路
    public List<AStarPoint> FindPath(AStarPoint mStartPoint, AStarPoint mEndPoint)
    {


        if (mEndPoint.MType == PointType.Obstacle || mStartPoint.MPosition == mEndPoint.MPosition)
        {

            return ShowPath(mStartPoint, mStartPoint);

        }

        //开启列表
        List<AStarPoint> openPointList = new List<AStarPoint>();
        //关闭列表
        List<AStarPoint> closePointList = new List<AStarPoint>();

        openPointList.Add(mStartPoint);

        //设一个bool是否有包含终点的点
        bool _isTarPoint = false;
        while (openPointList.Count > 0)
        {

            //寻找开启列表中最小预算值的表格
            AStarPoint minFPoint = FindPointWithMinF(openPointList);
            //将当前表格从开启列表移除 在关闭列表添加
            openPointList.Remove(minFPoint);
            closePointList.Add(minFPoint);
            //找到当前点周围的全部点
            List<AStarPoint> surroundPoints = FindSurroundPoint(minFPoint);

            ////被包围的情况下 无法行走
            //if (surroundPoints.Count <= 0)
            //{

            //    return ShowPath(mStartPoint, mStartPoint);

            //}

            //在周围的点中，将关闭列表里的点移除掉
            SurroundPointsFilter(surroundPoints, closePointList);
            //寻路逻辑
            foreach (AStarPoint surroundPoint in surroundPoints)
            {

                if (openPointList.Contains(surroundPoint))
                {

                    //计算下新路径下的G值（H值不变的，比较G相当于比较F值）
                    float newPathG = CalcG(surroundPoint, minFPoint);
                    if (newPathG < surroundPoint.MG)
                    {

                        surroundPoint.MG = newPathG;
                        surroundPoint.MF = surroundPoint.MG + surroundPoint.MH;
                        surroundPoint.MParentPoint = minFPoint;

                    }

                }
                else
                {

                    //将点之间的
                    surroundPoint.MParentPoint = minFPoint;
                    CalcF(surroundPoint, mEndPoint);
                    openPointList.Add(surroundPoint);

                }

            }

            //Debug.Log(openPointList.IndexOf(mEndPoint));
            //如果开始列表中包含了终点，说明找到路径
            if (openPointList.IndexOf(mEndPoint) > -1)
            {

                //此处可以添加行走不通的特效
                _isTarPoint = true;
                break;

            }

        }

        //如果没有终点
        if (!_isTarPoint)
        {

            mEndPoint = mStartPoint;

        }
        //测试生成路径的点

        return ShowPath(mStartPoint, mEndPoint);

    }


    List<AStarPoint> ShowPath(AStarPoint start, AStarPoint end)
    {

        mPathPosList.Clear();

        AStarPoint temp = end;
        while (true)
        {

            mPathPosList.Add(temp);

            //原White
            Color c = Color.white;
            //Color c = new Color(0f, 0.2f, 0.3f);

            if (temp == start)
            {

                //Green
                c = Color.green;

            }

            else if (temp == end)
            {

                //Red
                c = Color.red;

            }

            if (mPointGrid[temp.MPositionX, temp.MPositionY].MGameObject == null)
            {

                CreatePath(temp.MPositionX, temp.MPositionY, c);

            }

            if (temp.MParentPoint == null)
                break;

            //防止死循环
            if (temp.MPosition == start.MPosition)
            {

                break;

            }

            temp = temp.MParentPoint;

        }

        return mPathPosList;

    }


    void CreatePath(int _x, int _y, Color _color)
    {

        GameObject _obj;

        //Blue
        if (_color == Color.blue)
        {

            _obj = (GameObject)Object.Instantiate(Resources.Load("Items/P_Wall")) as GameObject;

        }
        else
        {

            _obj = (GameObject)Object.Instantiate(Resources.Load("P_PathLight")) as GameObject;
            //_obj = GameObject.CreatePrimitive(PrimitiveType.Plane);
            //go.GetComponent<MeshRenderer>().materials[0].shader = PathColor;
            //go.GetComponent<MeshRenderer>().materials[0].SetColor("_ShaderColor", color);
            _obj.GetComponent<SpriteRenderer>().color = new Color(_color.r, _color.g, _color.b, 0.382f);

        }

        _obj.transform.localScale = new Vector3(0.5f, 0.5f, 1);
        _obj.transform.SetParent(PathRoot);
        _obj.transform.localPosition = new Vector3(_x, _y, 0);

        if (Application.isEditor)
        {

            _obj.name = "路径";

        }

        if (mPointGrid[_x, _y].MGameObject != null)
        {

            if (mPointGrid[_x, _y].MType != PointType.TempThrough)
            {

                Object.Destroy(mPointGrid[_x, _y].MGameObject);

            }

        }
        mPointGrid[_x, _y].MGameObject = _obj;

    }


    //寻找预计值最小的格子
    AStarPoint FindPointWithMinF(List<AStarPoint> openPointList)
    {

        float f = float.MaxValue;

        AStarPoint temp = null;

        foreach (AStarPoint p in openPointList)
        {

            if (p.MF < f)
            {

                temp = p;
                f = p.MF;

            }

        }

        return temp;

    }


    //寻找周围的全部点
    List<AStarPoint> FindSurroundPoint(AStarPoint point)
    {

        List<AStarPoint> list = new List<AStarPoint>();

        ////////////判断周围的八个点是否在网格内/////////////
        AStarPoint up = null, down = null, left = null, right = null;

        if (point.MPositionY < MGridHeight - 1)
        {

            up = mPointGrid[point.MPositionX, point.MPositionY + 1];

        }
        if (point.MPositionY > 0)
        {

            down = mPointGrid[point.MPositionX, point.MPositionY - 1];

        }
        if (point.MPositionX > 0)
        {

            left = mPointGrid[point.MPositionX - 1, point.MPositionY];

        }
        if (point.MPositionX < MGridWidth - 1)
        {

            right = mPointGrid[point.MPositionX + 1, point.MPositionY];

        }

        /////////////将可以经过的表格添加到开启列表中/////////////
        if (down != null && down.MType != PointType.Obstacle)
        {

            list.Add(down);

        }
        if (up != null && up.MType != PointType.Obstacle)
        {

            list.Add(up);

        }
        if (left != null && left.MType != PointType.Obstacle)
        {

            list.Add(left);

        }
        if (right != null && right.MType != PointType.Obstacle)
        {

            list.Add(right);

        }
        if (Dir8)
        {

            AStarPoint lu = null, ru = null, ld = null, rd = null;

            if (up != null && left != null)
            {

                lu = mPointGrid[point.MPositionX - 1, point.MPositionY + 1];

            }
            if (up != null && right != null)
            {

                ru = mPointGrid[point.MPositionX + 1, point.MPositionY + 1];

            }
            if (down != null && left != null)
            {

                ld = mPointGrid[point.MPositionX - 1, point.MPositionY - 1];

            }
            if (down != null && right != null)
            {

                rd = mPointGrid[point.MPositionX + 1, point.MPositionY - 1];

            }

            if (lu != null && lu.MType != PointType.Obstacle && left.MType != PointType.Obstacle && up.MType != PointType.Obstacle)
            {

                list.Add(lu);

            }
            if (ld != null && ld.MType != PointType.Obstacle && left.MType != PointType.Obstacle && down.MType != PointType.Obstacle)
            {

                list.Add(ld);

            }
            if (ru != null && ru.MType != PointType.Obstacle && right.MType != PointType.Obstacle && up.MType != PointType.Obstacle)
            {

                list.Add(ru);

            }
            if (rd != null && rd.MType != PointType.Obstacle && right.MType != PointType.Obstacle && down.MType != PointType.Obstacle)
            {

                list.Add(rd);

            }

        }
        return list;

    }


    //将关闭带你从周围点列表中关闭
    void SurroundPointsFilter(List<AStarPoint> surroundPoints, List<AStarPoint> closePoints)
    {

        foreach (var closePoint in closePoints)
        {

            if (surroundPoints.Contains(closePoint))
            {

                //Debug.Log("将关闭列表的点移除");
                surroundPoints.Remove(closePoint);

            }

        }

    }


    //计算最小预算值点G值
    float CalcG(AStarPoint surround, AStarPoint minFPoint)
    {

        return Vector3.Distance(surround.MPosition, minFPoint.MPosition) + minFPoint.MG;

    }


    //计算该点到终点的F值
    void CalcF(AStarPoint now, AStarPoint end)
    {

        //F = G + H
        float h = Mathf.Abs(end.MPositionX - now.MPositionX) + Mathf.Abs(end.MPositionY - now.MPositionY);
        float g = now.MParentPoint == null ? 0 : Vector2.Distance(new Vector2(now.MPositionX, now.MPositionY), new Vector2(now.MParentPoint.MPositionX, now.MParentPoint.MPositionY)) + now.MParentPoint.MG;
        float f = g + h;
        now.MF = f;
        now.MG = g;
        now.MH = h;

    }

}
