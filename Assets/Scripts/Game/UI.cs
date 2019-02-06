using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;


public class UI
{

    //属性值
    public List<Text> AttrsValue;
    //图片
    public List<Sprite> EquipUIImages { get; set; }
    //图片
    public List<Sprite> ItemUIImages { get; set; }
    //按钮
    public List<Transform> ItemBtns { get; set; }
    public Transform ShopBtn { get; set; }
    public Transform ConfigBtn { get; set; }
    public Transform IAPBtn { get; set; }
    //UI
    public Transform WeaponUI { get; set; }
    public Transform ArmorUI { get; set; }
    public Text FloorText { get; set; }
    Sprite[] InfoUIImages { get; set; }
    public Transform MessagePart { get; set; }
    public Transform SelectsPart { get; set; }

    GameManager GM { get; set; }
    Canvas UICanvas { get; set; }
    public Transform RootUI { get; set; }
    //附带背景黑幕 点击关闭
    public Transform BasicUIBox { get; set; }
    public Transform ItemUIBox { get; set; }
    public Transform ShopUIBox { get; set; }
    public Transform ConfigUIBox { get; set; }
    Font font;


    Transform SaveSlotRoot { get; set; }


    class Supplie
    {

        public int _index = 0;
        public int _price = 0;
        public Sprite _sprite = null;
        public string _name = string.Empty;
        public string _info = string.Empty;
        public void Buy()
        {

            GameObject _contentItems = Object.Instantiate(Resources.Load<GameObject>("UI/ShopUIContent"));
            Button _button = _contentItems.transform.GetChild(_index).GetChild(0).GetComponent<Button>();
            if (SaveHeroInfo.Attributes[4] >= _price)
            {

                //可以购买

            }
            else
            {

                //无法购买
                Debug.Log("金币不足");

            }

        }

    }


    List<Supplie> Supplies { get; set; }


    void InitSupplies()
    {

        Supplies = new List<Supplie>();

        for (int _index = 0; _index < 12; _index++)
        {

            Supplies.Add(new Supplie());

        }

        for (int _index = 0; _index < Supplies.Count; _index++)
        {

            Supplies[_index]._index = _index;
            switch (_index)
            {

                case 0:
                    Supplies[_index]._price = 1000;
                    Supplies[_index]._sprite = Resources.LoadAll<Sprite>("Textures/Item01-01")[0];
                    Supplies[_index]._name = "黄钥匙";
                    Supplies[_index]._info = "打开黄门";
                    break;
                case 1:
                    Supplies[_index]._price = 1500;
                    Supplies[_index]._sprite = Resources.LoadAll<Sprite>("Textures/Item01-01")[1];
                    Supplies[_index]._name = "蓝钥匙";
                    Supplies[_index]._info = "打开蓝门";
                    break;
                case 2:
                    Supplies[_index]._price = 2000;
                    Supplies[_index]._sprite = Resources.LoadAll<Sprite>("Textures/Item01-01")[2];
                    Supplies[_index]._name = "红钥匙";
                    Supplies[_index]._info = "打开红门";
                    break;
                case 3:
                    Supplies[_index]._price = 1000;
                    Supplies[_index]._sprite = Resources.LoadAll<Sprite>("Textures/Item01-Gem01")[12];
                    Supplies[_index]._name = "红宝石";
                    Supplies[_index]._info = "+3攻击力";
                    break;
                case 4:
                    Supplies[_index]._price = 1000;
                    Supplies[_index]._sprite = Resources.LoadAll<Sprite>("Textures/Item01-Gem01")[13];
                    Supplies[_index]._name = "蓝宝石";
                    Supplies[_index]._info = "+3防御力";
                    break;
                case 5:
                    Supplies[_index]._price = 5000;
                    Supplies[_index]._sprite = Resources.LoadAll<Sprite>("UI/BaseIcons")[6];
                    Supplies[_index]._name = "存档位";
                    Supplies[_index]._info = "+1存档位";
                    break;
                case 6:
                    Supplies[_index]._price = 1000;
                    Supplies[_index]._sprite = Resources.LoadAll<Sprite>("Textures/Item01-02")[1];
                    Supplies[_index]._name = "大血瓶";
                    Supplies[_index]._info = "+500HP";
                    break;
                case 7:
                    Supplies[_index]._price = 10000;
                    Supplies[_index]._sprite = Resources.LoadAll<Sprite>("Textures/Item01-05")[3];
                    Supplies[_index]._name = "幸运金币";
                    Supplies[_index]._info = "杀怪得双倍金币";
                    break;
                case 8:
                    Supplies[_index]._price = 5000;
                    Supplies[_index]._sprite = Resources.LoadAll<Sprite>("Textures/Item01-06")[5];
                    Supplies[_index]._name = "向下飞行器";
                    Supplies[_index]._info = "往下飞1层";
                    break;
                case 9:
                    Supplies[_index]._price = 5000;
                    Supplies[_index]._sprite = Resources.Load<Sprite>("Textures/object_048");
                    Supplies[_index]._name = "筋斗云";
                    Supplies[_index]._info = "按中心对称飞行";
                    break;
                case 10:
                    Supplies[_index]._price = 5000;
                    Supplies[_index]._sprite = Resources.LoadAll<Sprite>("Textures/Item01-06")[8];
                    Supplies[_index]._name = "炸药";
                    Supplies[_index]._info = "消灭四周敌人";
                    break;
                case 11:
                    Supplies[_index]._price = 5000;
                    Supplies[_index]._sprite = Resources.LoadAll<Sprite>("Textures/Item01-06")[3];
                    Supplies[_index]._name = "地震卷轴";
                    Supplies[_index]._info = "销毁当前层的墙";
                    break;

            }

        }




    }


    public void Init()
    {

        font = (Font)Resources.Load("Fonts/Zpix") as Font;
        GM = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        UICanvas = GM.transform.gameObject.GetComponent<Canvas>();

        //属性 12个
        AttrsValue = new List<Text>();
        for (int _i = 0; _i < 12; _i++)
        {

            AttrsValue.Add(GameObject.FindWithTag("V" + _i).GetComponent<Text>());

        }

        EquipUIImages = new List<Sprite>
        {

            Resources.Load<Sprite>("UI/Empty"),
            Resources.LoadAll<Sprite>("Textures/Item01-08")[0],
            Resources.LoadAll<Sprite>("Textures/Item01-08")[5],
            Resources.LoadAll<Sprite>("Textures/Item01-08")[1],
            Resources.LoadAll<Sprite>("Textures/Item01-08")[6],
            Resources.LoadAll<Sprite>("Textures/Item01-08")[2],
            Resources.LoadAll<Sprite>("Textures/Item01-08")[7],
            Resources.LoadAll<Sprite>("Textures/Item01-08")[3],
            Resources.LoadAll<Sprite>("Textures/Item01-08")[8],
            Resources.LoadAll<Sprite>("Textures/Item01-08")[4],
            Resources.LoadAll<Sprite>("Textures/Item01-08")[9],
            Resources.Load<Sprite>("Textures/object_040"),
            Resources.Load<Sprite>("Textures/object_039")

        };

        ItemUIImages = new List<Sprite>
        {

            Resources.Load<Sprite>("Textures/object_017"),
            Resources.Load<Sprite>("Textures/object_043"),
            Resources.LoadAll<Sprite>("Textures/Item01-05")[9],
            Resources.LoadAll<Sprite>("Textures/Item01-03")[9],
            Resources.LoadAll<Sprite>("Textures/Item01-01")[4],
            Resources.LoadAll<Sprite>("Textures/Item01-06")[5],
            Resources.LoadAll<Sprite>("Textures/Item01-06")[6],
            Resources.LoadAll<Sprite>("Textures/Item01-05")[3],
            Resources.LoadAll<Sprite>("Textures/Item01-06")[8],
            Resources.LoadAll<Sprite>("Textures/Item01-06")[10],
            Resources.LoadAll<Sprite>("Textures/Item01-06")[2],
            Resources.Load<Sprite>("Textures/object_037"),
            Resources.Load<Sprite>("Textures/object_038"),
            Resources.LoadAll<Sprite>("Textures/Item01-06")[0],
            Resources.LoadAll<Sprite>("Textures/Item03-16")[0],
            Resources.Load<Sprite>("Textures/object_048")

        };
        WeaponUI = GameObject.FindWithTag("EP0").transform;
        ArmorUI = GameObject.FindWithTag("EP1").transform;

        //道具16个
        ItemBtns = new List<Transform>();
        Transform _itemBtnsRoot = GameObject.FindWithTag("ItemBtns").transform;
        for (int _i = 0; _i < 16; _i++)
        {

            _itemBtnsRoot.GetChild(_i).GetComponent<Button>().onClick.AddListener(GM.DoPropEvent);
            ItemBtns.Add(_itemBtnsRoot.GetChild(_i));

        }
        ShopBtn = GameObject.FindWithTag("ShopBtn").transform;
        ConfigBtn = GameObject.FindWithTag("ConfigBtn").transform;
        ShopBtn.GetComponent<Button>().onClick.AddListener(OpenMainUI);
        ConfigBtn.GetComponent<Button>().onClick.AddListener(OpenMainUI);

        RootUI = GameObject.FindWithTag("RootUI").transform;
        BasicUIBox = RootUI.GetChild(0);
        ItemUIBox = RootUI.GetChild(1);
        ShopUIBox = RootUI.GetChild(2);
        ConfigUIBox = RootUI.GetChild(3);

        BasicUIBox.GetComponent<Button>().onClick.AddListener(GM.CloseUIBox);
        ItemUIBox.GetComponent<Button>().onClick.AddListener(GM.CloseUIBox);
        ShopUIBox.GetComponent<Button>().onClick.AddListener(GM.CloseUIBox);
        ConfigUIBox.GetComponent<Button>().onClick.AddListener(GM.CloseUIBox);
        //有个安全框 SafeFrame
        MessagePart = BasicUIBox.GetChild(0).GetChild(0);
        SelectsPart = BasicUIBox.GetChild(0).GetChild(1);

        FloorText = GameObject.FindWithTag("F").GetComponent<Text>();

        InitSupplies();

    }

    #region 公用方法 Open、Create
    /// <summary>
    /// 是否全部UI已关闭
    /// </summary>
    /// <returns><c>true</c>, if is active was user interfaced, <c>false</c> otherwise.</returns>
    public bool IsAllUIClosed()
    {

        for (int _i = 0; _i < RootUI.childCount; _i++)
        {

            if (RootUI.GetChild(_i).gameObject.activeSelf)
            {

                return false;

            }

        }
        return true;

    }


    /// <summary>
    /// 是否是升级雕像 | 商店
    /// </summary>
    /// <returns><c>true</c>, if shop was ised, <c>false</c> otherwise.</returns>
    /// <param name="_id">hero_selet id.</param>
    public bool IsShop(int _id, bool _ispropertychangeid = false)
    {

        //是PropertyChange Id
        if (_ispropertychangeid)
        {

            foreach (int e in GM.ShopPropertyChangeIds)
            {

                if (e == _id)
                {

                    return true;

                }

            }

        }
        else
        {

            //是hero_select Id
            return _id >= 15 && _id <= 20;

        }

        return false;


    }
    #endregion


    /// <summary>
    /// HeroInfo界面上的按钮的功能
    /// </summary>
    void OpenMainUI()
    {

        Time.timeScale = 0;
        GameObject _buttonSelf = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
        //Debug.Log(buttonSelf.name);
        if (_buttonSelf == ShopBtn.gameObject)
        {

            Transform _content = ShopUIBox.GetChild(1).GetChild(1);
            if (_content.childCount == 0)
            {

                GameObject _contentItems = Object.Instantiate(Resources.Load<GameObject>("UI/ShopUIContent"));
                _contentItems.transform.SetParent(_content);
                _contentItems.GetComponent<RectTransform>().SetAnchor(AnchorPresets.StretchAll);
                _contentItems.GetComponent<RectTransform>().offsetMin = Vector2.zero;
                _contentItems.GetComponent<RectTransform>().offsetMax = Vector2.zero;

                //初始化ShopItem
                for (int _itemIndex = 0; _itemIndex < _contentItems.transform.childCount; _itemIndex++)
                {

                    _contentItems.transform.GetChild(_itemIndex).GetChild(0).GetChild(0).GetChild(1).GetComponent<Text>().text = Supplies[_itemIndex]._price.ToString();
                    _contentItems.transform.GetChild(_itemIndex).GetChild(0).GetChild(1).GetChild(0).GetComponent<Image>().sprite = Supplies[_itemIndex]._sprite;
                    _contentItems.transform.GetChild(_itemIndex).GetChild(0).GetChild(2).GetChild(0).GetComponent<Text>().text = Supplies[_itemIndex]._name;
                    _contentItems.transform.GetChild(_itemIndex).GetChild(0).GetChild(2).GetChild(1).GetComponent<Text>().text = Supplies[_itemIndex]._info;
                    _contentItems.transform.GetChild(_itemIndex).GetChild(0).GetComponent<Button>().onClick.AddListener(Supplies[_itemIndex].Buy);

                }
                //ShopItem初始化完毕

            }
            if (IAPBtn == null)
            {

                IAPBtn = ShopUIBox.GetChild(1).GetChild(2).GetChild(0);
                IAPBtn.GetComponent<Button>().onClick.AddListener(OpenMainUI);

            }
            ShopUIBox.gameObject.SetActive(true);

        }
        if (_buttonSelf == ConfigBtn.gameObject)
        {

            //Config盒子打开
            ConfigUIBox.gameObject.SetActive(true);
            //Btn显示
            ConfigUIBox.GetChild(0).GetChild(0).gameObject.SetActive(true);
            //初始化Config的Btns打开
            if (ConfigUIBox.GetChild(0).GetChild(0).childCount == 0)
            {

                Transform _configBtns = Object.Instantiate(Resources.Load<GameObject>("UI/ConfigBtns")).transform;
                _configBtns.SetParent(ConfigUIBox.GetChild(0).GetChild(0));
                _configBtns.GetComponent<RectTransform>().SetAnchor(AnchorPresets.StretchAll);
                _configBtns.GetComponent<RectTransform>().offsetMin = Vector2.zero;
                _configBtns.GetComponent<RectTransform>().offsetMax = Vector2.zero;
                for (int _index = 0; _index < _configBtns.childCount; _index++)
                {

                    _configBtns.GetChild(_index).GetComponent<Button>().onClick.AddListener(OpenConfigBtnUI);

                }

            }

            //UI隐藏
            if (ConfigUIBox.GetChild(0).GetChild(1).childCount != 0)
            {

                Transform ConfigUIs = ConfigUIBox.GetChild(0).GetChild(1).GetChild(0);
                for (int _index = 0; _index < ConfigUIs.childCount; _index++)
                {

                    ConfigUIs.GetChild(_index).gameObject.SetActive(false);

                }

            }

        }

    }


    /// <summary>
    /// Config中按钮打开的UI
    /// </summary>
    void OpenConfigBtnUI()
    {

        GameObject _buttonSelf = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;

        Transform _configBtns = ConfigUIBox.GetChild(0).GetChild(0).GetChild(0);
        Transform _configUIs;

        //初始化ConfigUI
        if (ConfigUIBox.GetChild(0).GetChild(1).childCount == 0)
        {

            _configUIs = Object.Instantiate(Resources.Load<GameObject>("UI/ConfigUIs")).transform;
            _configUIs.SetParent(ConfigUIBox.GetChild(0).GetChild(1));
            _configUIs.GetComponent<RectTransform>().SetAnchor(AnchorPresets.StretchAll);
            _configUIs.GetComponent<RectTransform>().offsetMin = Vector2.zero;
            _configUIs.GetComponent<RectTransform>().offsetMax = Vector2.zero;

        }
        else
        {

            _configUIs = ConfigUIBox.GetChild(0).GetChild(1).GetChild(0);

        }

        _configBtns.parent.gameObject.SetActive(false);
        for (int _tarIndex = 0; _tarIndex < _configBtns.childCount; _tarIndex++)
        {

            if (_buttonSelf == _configBtns.GetChild(_tarIndex).gameObject)
            {

                for (int _index = 0; _index < _configUIs.childCount; _index++)
                {

                    _configUIs.GetChild(_index).gameObject.SetActive(false);

                }
                _configUIs.GetChild(_tarIndex).gameObject.SetActive(true);

                switch (_tarIndex)
                {

                    case 0://打开存储界面
                        if (SaveSlotRoot == null)
                        {

                            SaveSlotRoot = _configUIs.GetChild(_tarIndex).GetChild(1);

                        }

                        int _saveSlotsCount = Dict.GetColCount(Dict.SqlDBName, "z_save_base_info");
                        for (int _slotIndex = 0; _slotIndex < _saveSlotsCount; _slotIndex++)
                        {

                            //0: 未解锁 1: 已解锁但为空存档 2: 已解锁并有存储记录 3: 自动存档
                            int _value = Dict.GetInt(Dict.SqlDBName, "z_save_base_info", "value", _slotIndex + 1);
                            switch (_value)
                            {

                                case 0:
                                    SaveSlotRoot.GetChild(_slotIndex).GetChild(0).GetComponent<Text>().text = "[未解锁]";
                                    SaveSlotRoot.GetChild(_slotIndex).GetComponent<Image>().raycastTarget = false;
                                    SaveSlotRoot.GetChild(_slotIndex).GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f);
                                    break;
                                case 1:
                                    SaveSlotRoot.GetChild(_slotIndex).GetChild(0).GetComponent<Text>().text = "[空]";
                                    SaveSlotRoot.GetChild(_slotIndex).GetComponent<Image>().raycastTarget = false;
                                    SaveSlotRoot.GetChild(_slotIndex).GetComponent<Image>().color = Color.white;
                                    SaveSlotRoot.GetChild(_slotIndex).GetComponent<Button>().onClick.RemoveAllListeners();
                                    SaveSlotRoot.GetChild(_slotIndex).GetComponent<Button>().onClick.AddListener(Save);
                                    break;
                                case 2:
                                    SaveSlotRoot.GetChild(_slotIndex).GetChild(0).GetComponent<Text>().text = Dict.GetAllDict()["System"]["z_save_base_info"]["year"][_slotIndex + 1] + "/"
                                                                                                        + Dict.GetAllDict()["System"]["z_save_base_info"]["month"][_slotIndex + 1] + "/"
                                                                                                        + Dict.GetAllDict()["System"]["z_save_base_info"]["day"][_slotIndex + 1] + " "
                                                                                                        + Dict.GetAllDict()["System"]["z_save_base_info"]["hour"][_slotIndex + 1] + ":"
                                                                                                        + Dict.GetAllDict()["System"]["z_save_base_info"]["minute"][_slotIndex + 1] + ":"
                                                                                                        + Dict.GetAllDict()["System"]["z_save_base_info"]["second"][_slotIndex + 1];
                                    SaveSlotRoot.GetChild(_slotIndex).GetComponent<Image>().raycastTarget = true;
                                    SaveSlotRoot.GetChild(_slotIndex).GetComponent<Image>().color = Color.white;
                                    SaveSlotRoot.GetChild(_slotIndex).GetComponent<Button>().onClick.RemoveAllListeners();
                                    SaveSlotRoot.GetChild(_slotIndex).GetComponent<Button>().onClick.AddListener(Save);
                                    break;
                                case 3:
                                    SaveSlotRoot.GetChild(_slotIndex).GetChild(0).GetComponent<Text>().text = "[自动存档位]" + " - "
                                                                                                        + Dict.GetAllDict()["System"]["z_save_base_info"]["year"][_slotIndex + 1] + "/"
                                                                                                        + Dict.GetAllDict()["System"]["z_save_base_info"]["month"][_slotIndex + 1] + "/"
                                                                                                        + Dict.GetAllDict()["System"]["z_save_base_info"]["day"][_slotIndex + 1] + " "
                                                                                                        + Dict.GetAllDict()["System"]["z_save_base_info"]["hour"][_slotIndex + 1] + ":"
                                                                                                        + Dict.GetAllDict()["System"]["z_save_base_info"]["minute"][_slotIndex + 1] + ":"
                                                                                                        + Dict.GetAllDict()["System"]["z_save_base_info"]["second"][_slotIndex + 1];
                                    SaveSlotRoot.GetChild(_slotIndex).GetComponent<Image>().raycastTarget = false;
                                    SaveSlotRoot.GetChild(_slotIndex).GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f);
                                    break;

                            }

                        }
                        break;
                    case 1://声音设置
                        Button _musicCtrBtn = _configUIs.GetChild(_tarIndex).GetChild(1).GetChild(1).GetComponent<Button>();
                        Button _soundCtrBtn = _configUIs.GetChild(_tarIndex).GetChild(1).GetChild(2).GetComponent<Button>();
                        _musicCtrBtn.onClick.RemoveAllListeners();
                        _soundCtrBtn.onClick.RemoveAllListeners();
                        _musicCtrBtn.onClick.AddListener(GM.MyCommon.MusicSwitch);
                        _soundCtrBtn.onClick.AddListener(GM.MyCommon.SoundSwitch);
                        if (Dict.GetInt(Dict.SqlDBName, "system", "is_music_on", 1) == 1)
                        {

                            _musicCtrBtn.GetComponent<Image>().color = new Color(1, 1, 1, 0);

                        }
                        if (Dict.GetInt(Dict.SqlDBName, "system", "is_sound_on", 1) == 1)
                        {

                            _soundCtrBtn.GetComponent<Image>().color = new Color(1, 1, 1, 0);

                        }
                        break;
                    case 3://回主界面
                           //初始化
                        if (_configUIs.GetChild(_tarIndex).childCount == 0)
                        {

                            GameObject _choiceBox = Object.Instantiate(Resources.Load<GameObject>("UI/NormalChoiceBoxSafeFrame"));
                            _choiceBox.transform.SetParent(_configUIs.GetChild(_tarIndex));
                            _choiceBox.GetComponent<RectTransform>().SetAnchor(AnchorPresets.StretchAll);
                            _choiceBox.GetComponent<RectTransform>().offsetMin = Vector2.zero;
                            _choiceBox.GetComponent<RectTransform>().offsetMax = Vector2.zero;
                            _choiceBox.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Text>().text = "未储存的数据将丢失，" + "\n" + "确定吗？";
                            Button _btnYes = _choiceBox.transform.GetChild(0).GetChild(1).GetChild(0).GetComponent<Button>();
                            Button _btnNo = _choiceBox.transform.GetChild(0).GetChild(1).GetChild(1).GetComponent<Button>();
                            _btnYes.onClick.AddListener(() => SceneManager.LoadScene(1));
                            _btnNo.onClick.AddListener(GM.CloseUIBox);

                        }
                        break;
                    case 4://继续游戏
                        GM.CloseUIBox();
                        break;

                }

            }

        }

    }


    void Save()
    {

        GameObject _buttonSelf = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
        //id 0 为初始化用DB文件
        for (int _index = 0; _index < SaveSlotRoot.childCount; _index++)
        {

            if (_buttonSelf == SaveSlotRoot.GetChild(_index).gameObject)
            {

                Dict.SaveId = _index + 1;
                Dictionary<string, int> _time = new Dictionary<string, int>
                {

                    {"year",System.DateTime.Now.Year},
                    {"month",System.DateTime.Now.Month},
                    {"day",System.DateTime.Now.Day},
                    {"hour",System.DateTime.Now.Hour},
                    {"minute",System.DateTime.Now.Minute},
                    {"second",System.DateTime.Now.Second}

                };
                foreach (string _key in _time.Keys)
                {

                    Dict.SetInt(Dict.SqlDBName, "z_save_base_info", _key, _index + 1, _time[_key]);

                }
                SaveSlotRoot.GetChild(_index).GetChild(0).GetComponent<Text>().text = _time["year"] + "/" + _time["month"] + "/" + _time["day"] + " " + _time["hour"] + ":" + _time["minute"] + ":" + _time["second"];
                Dict.SetInt(Dict.SqlDBName, "z_save_base_info", "value", Dict.SaveId, 2);
                Debug.Log("保存中...");
                DBDataBase.DictBase.SaveToSaveDb(Dict.SaveId);
                DBDataBase.DictBase.UpdateDBToDict(Dict.SaveDBName);
                Debug.Log("保存完毕 !");
                return;

            }

        }

    }


    //void MusicSwitch()
    //{

    //    string _col = "is_music_on";
    //    int _value = Dict.GetInt(Dict.SqlDBName, "system", _col, 1);
    //    //当前为打开
    //    if (_value == 0)
    //    {

    //        Dict.SetInt(Dict.SqlDBName, "system", _col, 1, 1);
    //        ConfigUIBox.GetChild(0).GetChild(1).GetChild(0).GetChild(1).GetChild(1).GetChild(1).GetComponent<Image>().color = new Color(1, 1, 1, 0);
    //        foreach (AudioSource _s in GM.MyCommon.MusicSource)
    //        {

    //            if (_s == GM.MyCommon.MusicSource[7] || _s == GM.MyCommon.MusicSource[8] || _s == GM.MyCommon.MusicSource[9])
    //            {

    //                _s.mute = true;

    //            }

    //        }

    //    }
    //    //当前为关闭
    //    else
    //    {

    //        Dict.SetInt(Dict.SqlDBName, "system", _col, 1, 0);
    //        ConfigUIBox.GetChild(0).GetChild(1).GetChild(0).GetChild(1).GetChild(1).GetChild(1).GetComponent<Image>().color = new Color(1, 1, 1, 1);
    //        foreach (AudioSource _s in GM.MyCommon.MusicSource)
    //        {

    //            if (_s == GM.MyCommon.MusicSource[7] || _s == GM.MyCommon.MusicSource[8] || _s == GM.MyCommon.MusicSource[9])
    //            {

    //                _s.mute = false;

    //            }

    //        }

    //    }

    //}
    //void SoundSwitch()
    //{

    //    string _col = "is_sound_on";
    //    int _value = Dict.GetInt(Dict.SqlDBName, "system", _col, 1);
    //    //当前为打开
    //    if (_value == 0)
    //    {

    //        Dict.SetInt(Dict.SqlDBName, "system", _col, 1, 1);
    //        ConfigUIBox.GetChild(0).GetChild(1).GetChild(0).GetChild(1).GetChild(1).GetChild(2).GetComponent<Image>().color = new Color(1, 1, 1, 0);
    //        foreach (AudioSource _s in GM.MyCommon.MusicSource)
    //        {

    //            if (_s == GM.MyCommon.MusicSource[7] || _s == GM.MyCommon.MusicSource[8] || _s == GM.MyCommon.MusicSource[9])
    //            {

    //                continue;

    //            }
    //            _s.mute = true;

    //        }

    //    }
    //    //当前为关闭
    //    else
    //    {

    //        Dict.SetInt(Dict.SqlDBName, "system", _col, 1, 0);
    //        ConfigUIBox.GetChild(0).GetChild(1).GetChild(0).GetChild(1).GetChild(1).GetChild(2).GetComponent<Image>().color = new Color(1, 1, 1, 1);
    //        foreach (AudioSource _s in GM.MyCommon.MusicSource)
    //        {

    //            if (_s == GM.MyCommon.MusicSource[7] || _s == GM.MyCommon.MusicSource[8] || _s == GM.MyCommon.MusicSource[9])
    //            {

    //                continue;

    //            }
    //            _s.mute = false;

    //        }

    //    }

    //}


    #region 初始化init
    public void FloorJumpDown()
    {

        //if (Application.isEditor)
        //{

        //    GM.EnterMapId(SaveHeroInfo.CurMapId - 1, true);

        //}
        //else
        //{

        //最低为最小到达的楼层
        if (SaveHeroInfo.CurMapId >= (SaveHeroInfo.ReachFloorIdLimit.x - 1))
        {

            //最低0层 但不可用跳跃器到达
            int _tarMapId = SaveHeroInfo.CurMapId - 1 >= 1 ? SaveHeroInfo.CurMapId - 1 : 1;
            //第零层:id 1 第四十四层:id 45 不可直接到达
            if (_tarMapId == 45)
            {

                _tarMapId = 44;

            }

            if (_tarMapId == 1)
            {

                return;

            }

            GM.EnterMapId(_tarMapId, true);
            //音效
            GM.MyCommon.MusicSource[6].Play();
            MessagePart.GetComponentInChildren<Text>().text = "当前楼层 F" + (SaveHeroInfo.CurMapId - 1) + "\n" + "\n" + "您的选择是？";

        }

        //}

    }


    public void FloorJumpUp()
    {

        //if (Application.isEditor)
        //{

        //    GM.EnterMapId(SaveHeroInfo.CurMapId + 1, true);

        //}
        //else
        //{

        //最高为最高到达的楼层
        if (SaveHeroInfo.CurMapId <= (SaveHeroInfo.ReachFloorIdLimit.y - 1))
        {

            int _tarMapId = SaveHeroInfo.CurMapId + 1 <= 68 ? SaveHeroInfo.CurMapId + 1 : 68;
            //1 第四十四层:id 45 不可直接到达
            if (_tarMapId == 45)
            {

                _tarMapId = 46;

            }

            GM.EnterMapId(_tarMapId, true);
            //音效
            GM.MyCommon.MusicSource[6].Play();
            MessagePart.GetComponentInChildren<Text>().text = "当前楼层 F" + (SaveHeroInfo.CurMapId - 1) + "\n" + "\n" + "您的选择是？";

        }

        //}

    }


    /// <summary>
    /// 圣水的功能
    /// </summary>
    public void HolyWater()
    {

        //增加生命值 = ( 攻 + 防 ) * 10;
        int _increase = (SaveHeroInfo.Attributes[2] + SaveHeroInfo.Attributes[3]) * 10;
        GM.DoChange(1, _increase);
        //0 出现 1 为消失
        SaveHeroInfo.Props[3] = 1;
        SaveHeroInfo.RefreshUI();
        GM.CloseUIBox();

    }


    /// <summary>
    /// 黄金钥匙的功能
    /// </summary>
    public void GoldKey()
    {

        string _dbName = Dict.SqlDBName;
        string _tabName = "map_obj";

        int _mapWidth = Dict.MapWidth;
        int _mapHeight = Dict.MapHeight;
        for (int _x = 0; _x < _mapWidth; _x++)
        {

            for (int _y = 0; _y < _mapHeight; _y++)
            {

                if (GM.MPointGrid[_x, _y].IsObj)
                {

                    //MapObj的TypeId == 3 即黄门
                    int _mapObjId = GM.MPointGrid[_x, _y].MMapObjId;
                    if (_mapObjId <= 0)
                    {

                        continue;

                    }
                    if (Dict.GetInt(_dbName, _tabName, "type_id", _mapObjId) == 3)
                    {

                        //0 显示  1 消失
                        Dict.SetInt(_dbName, _tabName, "value", _mapObjId, 1);
                        GM.MPointGrid[_x, _y].UpdateSelf();
                        ////保存Map Obj Id数据
                        //GM.SaveMapObjById(GM.MPointGrid[_x, _y].MMapObjId);

                    }

                }

            }

        }
        SaveHeroInfo.Props[4] = 1;
        SaveHeroInfo.RefreshUI();
        GM.CloseUIBox();

    }


    /// <summary>
    /// 向下飞行器
    /// </summary>
    public void FloorDown()
    {

        //type id == 6  向下飞行器
        int _tarMapId = SaveHeroInfo.CurMapId - 1 >= 1 ? SaveHeroInfo.CurMapId - 1 : 1;
        GM.EnterMapId(_tarMapId);

        SaveHeroInfo.Props[5] = 1;
        SaveHeroInfo.RefreshUI();
        GM.CloseUIBox();

    }


    /// <summary>
    /// 向上飞行器
    /// </summary>
    public void FloorUp()
    {

        //type id == 7  向上飞行器
        int _tarMapId = SaveHeroInfo.CurMapId + 1 <= 68 ? SaveHeroInfo.CurMapId + 1 : 68;
        GM.EnterMapId(_tarMapId);

        SaveHeroInfo.Props[6] = 1;
        SaveHeroInfo.RefreshUI();
        GM.CloseUIBox();

    }


    /// <summary>
    /// 炸药
    /// </summary>
    public void Explosive()
    {

        string _dbName = Dict.SqlDBName;
        string _tabName = "map_obj";

        //需要遍历的位置
        List<Vector2> _tempPos = new List<Vector2>
        {

            GM.MStartPos.MPosition + Vector2Int.left,
            GM.MStartPos.MPosition + Vector2Int.right,
            GM.MStartPos.MPosition + Vector2Int.up,
            GM.MStartPos.MPosition + Vector2Int.down

        };

        //去除超出边界的点
        foreach (Vector2 _e in _tempPos)
        {

            //限定边界之内
            if (_e.x >= 0 && _e.x <= 10 && _e.y >= 0 && _e.y <= 10)
            {

                int _mapObjId = GM.MPointGrid[Mathf.RoundToInt(_e.x), Mathf.RoundToInt(_e.y)].MMapObjId;
                if (_mapObjId == 0)
                {

                    continue;

                }

                //MapObj TypeId == 14 为怪物
                if (Dict.GetInt(_dbName, _tabName, "type_id", _mapObjId) == 14)
                {

                    //MapObj MiniTypeId 为普通怪物 可以炸
                    switch (Dict.GetInt(_dbName, _tabName, "mini_type_id", _mapObjId))
                    {

                        case 1://为普通怪物
                        case 2:
                        case 3:
                        case 4:
                        case 5:
                        case 6:
                        case 7:
                        case 8:
                        case 10:
                        case 11:
                        case 12:
                        case 13:
                        case 14:
                        case 16:
                        case 17:
                        case 18:
                        case 19:
                        case 21:
                        case 22:
                        case 24:
                        case 25:
                        case 26:
                        case 27:
                        case 28:
                        case 29:
                        case 35:
                        case 38:
                        case 39:
                            //清除 前后左右 的普通怪物
                            //0存在 1消失
                            Dict.SetInt(_dbName, _tabName, "value", GM.MPointGrid[Mathf.RoundToInt(_e.x), Mathf.RoundToInt(_e.y)].MMapObjId, 1);
                            //刷新点状态
                            GM.MPointGrid[Mathf.RoundToInt(_e.x), Mathf.RoundToInt(_e.y)].UpdateSelf();
                            ////保存Map Obj Id数据
                            //GM.SaveMapObjById(GM.MPointGrid[Mathf.RoundToInt(_e.x), Mathf.RoundToInt(_e.y)].MMapObjId);
                            break;

                        case 9://为Boss
                        case 15:
                        case 20:
                        case 23:
                        case 30:
                        case 31:
                        case 32:
                        case 33:
                        case 34:
                        case 36:
                        case 37:
                        case 40:
                            break;

                    }

                }


            }

        }

        SaveHeroInfo.Props[8] = 1;
        SaveHeroInfo.RefreshUI();
        GM.CloseUIBox();

    }


    /// <summary>
    /// 避水珠
    /// </summary>
    public void BiShuiZhu()
    {
        //不用 待删
    }


    /// <summary>
    /// 芭蕉扇
    /// </summary>
    public void BaJiaoShan()
    {

        string _dbName = Dict.SqlDBName;
        string _tabName = "map_obj";

        int _mapWidth = Dict.MapWidth;
        int _mapHeight = Dict.MapHeight;
        for (int _x = 0; _x < _mapWidth; _x++)
        {

            for (int _y = 0; _y < _mapHeight; _y++)
            {

                if (GM.MPointGrid[_x, _y].IsObj)
                {

                    int _mapObjId = GM.MPointGrid[_x, _y].MMapObjId;
                    if (_mapObjId == 0)
                    {

                        continue;

                    }
                    //MapObj的TypeId == 70 即火焰山
                    if (Dict.GetInt(_dbName, _tabName, "type_id", _mapObjId) == 70)
                    {

                        //0 显示  1 消失
                        Dict.SetInt(_dbName, _tabName, "value", _mapObjId, 1);
                        GM.MPointGrid[_x, _y].UpdateSelf();
                        ////保存Map Obj Id数据
                        //GM.SaveMapObjById(GM.MPointGrid[Mathf.RoundToInt(_x), Mathf.RoundToInt(_y)].MMapObjId);

                    }

                }

            }

        }
        SaveHeroInfo.Props[11] = 1;
        SaveHeroInfo.RefreshUI();
        GM.CloseUIBox();

    }


    /// <summary>
    /// 猴毛
    /// </summary>
    public void HouMao()
    {

        if (SaveHeroInfo.CurMapId != 61)
        {

            return;

        }

        string _dbName = Dict.SqlDBName;
        string _tabName = "map_obj";

        int _mapWidth = Dict.MapWidth;
        int _mapHeight = Dict.MapHeight;

        bool _attack = false;
        for (int _x = 0; _x < _mapWidth; _x++)
        {

            for (int _y = 0; _y < _mapHeight; _y++)
            {

                if (GM.MPointGrid[_x, _y].IsObj)
                {

                    //是怪物
                    int _mapObjId = GM.MPointGrid[_x, _y].MMapObjId;
                    if (_mapObjId == 0)
                    {

                        continue;

                    }
                    //MapObj的TypeId == 14 即怪物
                    if (Dict.GetInt(_dbName, _tabName, "type_id", _mapObjId) == 14)
                    {

                        int _tarX = Dict.GetInt(_dbName, _tabName, "pos_x", _mapObjId);
                        int _tarY = Dict.GetInt(_dbName, _tabName, "pos_y", _mapObjId) - 1;

                        //如果位置为空
                        if (GM.MPointGrid[_tarX, _tarY].MGameObject == null)
                        {

                            Sprite _sprite = Resources.LoadAll<Sprite>("Textures/Actor01")[12];
                            GM.GetMap().CreateMapObj(GM.GetMap().Path.transform, _sprite, _tarX, _tarY);
                            //保存Map Obj Id数据
                            //GM.SaveMapObjById(GM.MPointGrid[Mathf.RoundToInt(_tarY), Mathf.RoundToInt(_tarY)].MMapObjId);

                        }
                        //如果位置不为空
                        else
                        {

                            GM.DestroyMapObj(GM.MPointGrid[_x, _y]);
                            GM.DestroyMapObj(GM.MPointGrid[_tarX, _tarY]);
                            GM.UpdateSwitch(497);
                            SaveHeroInfo.Props[12] = 1;
                            SaveHeroInfo.RefreshUI();
                            _attack = true;
                            //保存Map Obj Id数据
                            //GM.SaveMapObjById(GM.MPointGrid[Mathf.RoundToInt(_tarY), Mathf.RoundToInt(_tarY)].MMapObjId);

                        }

                    }

                }

            }

        }
        if (_attack)
        {

            Object.Instantiate(Resources.Load<Object>("VFXs/hit-white-1"), GM.MStartPos.MPosition, Quaternion.identity);
            //音效
            GM.MyCommon.MusicSource[0].Play();

        }
        GM.CloseUIBox();

    }


    /// <summary>
    /// 铁镐
    /// </summary>
    public void Mattock()
    {

        List<Vector2> _tempPos = new List<Vector2>
        {

            GM.MStartPos.MPosition + Vector2Int.left,
            GM.MStartPos.MPosition + Vector2Int.right,
            GM.MStartPos.MPosition + Vector2Int.up,
            GM.MStartPos.MPosition + Vector2Int.down

        };

        foreach (Vector2 _e in _tempPos)
        {

            if (_e.x < 0 || _e.x > (Dict.MapWidth - 1) || _e.y < 0 || _e.y > (Dict.MapHeight - 1))
            {

                continue;

            }
            if (GM.MPointGrid[Mathf.RoundToInt(_e.x), Mathf.RoundToInt(_e.y)].IsWall)
            {

                GM.MPointGrid[Mathf.RoundToInt(_e.x), Mathf.RoundToInt(_e.y)].Clear();

            }

        }

        SaveHeroInfo.Props[13] = 1;
        SaveHeroInfo.RefreshUI();
        GM.CloseUIBox();

    }


    /// <summary>
    /// 地震卷轴
    /// </summary>
    public void ShakeScroll()
    {

        //string _dbName = Dict.SqlDBName;

        int _mapWidth = Dict.MapWidth;
        int _mapHeight = Dict.MapHeight;

        string _dbName = "";
        string _tabName = "";

        //暗墙和彩蛋墙全部移除
        for (int _x = 0; _x < _mapWidth; _x++)
        {

            for (int _y = 0; _y < _mapHeight; _y++)
            {

                if (GM.MPointGrid[_x, _y].IsWall)
                {

                    _dbName = Dict.WallDBName;
                    _tabName = "map_" + SaveHeroInfo.CurMapId;
                    //普通墙体全部移除
                    Dict.SetInt(_dbName, _tabName, "value", GM.MPointGrid[_x, _y].MWallId, 1);
                    GM.MPointGrid[_x, _y].UpdateSelf();

                }

                if (GM.MPointGrid[_x, _y].IsObj && GM.MPointGrid[_x, _y].MMapObjId != 0)
                {

                    _dbName = Dict.SqlDBName;
                    _tabName = "map_obj";
                    int _mapObjTypeId = Dict.GetInt(_dbName, _tabName, "type_id", GM.MPointGrid[_x, _y].MMapObjId);
                    _tabName = "map_obj_type";
                    if (Dict.GetInt(_dbName, _tabName, "id", _mapObjTypeId) == 67
                    || Dict.GetInt(_dbName, _tabName, "id", _mapObjTypeId) == 72
                    || Dict.GetInt(_dbName, _tabName, "id", _mapObjTypeId) == 73
                    || Dict.GetInt(_dbName, _tabName, "id", _mapObjTypeId) == 74
                    || Dict.GetInt(_dbName, _tabName, "id", _mapObjTypeId) == 75
                    )
                    {

                        Dict.SetInt(_dbName, "map_obj", "value", GM.MPointGrid[_x, _y].MMapObjId, 1);

                    }
                    GM.MPointGrid[_x, _y].UpdateSelf();

                }

            }

        }
        SaveHeroInfo.Props[14] = 1;
        SaveHeroInfo.RefreshUI();
        GM.CloseUIBox();

    }


    /// <summary>
    /// 筋斗云
    /// </summary>
    public void Jindowin()
    {

        //无

    }
    #endregion

}







//暂时没用到
//public void RefreshAll()
//{

//    int index = 0;
//    foreach (Text t in Attributes)
//    {

//        t.text = SaveHeroInfo.Attributes[index] + " ";
//        index++;

//    }
//    Debug.Log("刷新完毕");

//}