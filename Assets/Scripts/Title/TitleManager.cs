using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{

    Transform BtnRoot { get; set; }
    //不包含SafeFrame
    Transform UIRoot { get; set; }
    Common MyCommon { get; set; }


    Transform SaveSlotRoot { get; set; }


    Transform[] _introPages { get; set; }
    Transform[] _introBtns { get; set; }
    int _introPageIndex { get; set; }


    // Use this for initialization
    void Start()
    {


        if (GameObject.FindWithTag("Common") == null)
        {

            SceneManager.LoadScene(0);
            return;

        }

        MyCommon = GameObject.FindWithTag("Common").GetComponent<Common>();
        MyCommon.PlayBGM(7);
        UIRoot = transform.GetChild(1);
        UIRoot.GetComponent<Button>().onClick.AddListener(CloseTitleUI);


        //主界面背景图
        transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>("UI/Bg" + Dict.GetInt(Dict.SqlDBName, "system", "title_bg", 1));

        SaveSlotRoot = UIRoot.GetChild(0).GetChild(1).GetChild(1);

        //Intro UI页初始化
        Transform _introPageRoot = UIRoot.GetChild(0).GetChild(3).GetChild(1).GetChild(0);
        _introPages = new Transform[]
        {

            _introPageRoot.GetChild(0),
            _introPageRoot.GetChild(1),
            _introPageRoot.GetChild(2)

        };
        _introBtns = new Transform[]
        {

            _introPageRoot.parent.GetChild(1),_introPageRoot.parent.GetChild(2)

        };
        //左右箭头按钮
        for (int _index = 0; _index < _introBtns.Length; _index++)
        {

            _introBtns[_index].GetComponent<Button>().onClick.AddListener(UpdateIntroPage);

        }

        //Title Bg
        transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>("UI/Bg" + Dict.GetInt(Dict.SqlDBName, "system", "title_bg", 1));
        BtnRoot = transform.GetChild(0).GetChild(2);
        for (int _index = 0; _index < BtnRoot.childCount; _index++)
        {

            BtnRoot.GetChild(_index).GetComponent<Button>().onClick.AddListener(DoEvent);

        }

        CloseTitleUI();

    }


    void DoEvent()
    {

        GameObject _buttonSelf = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
        UIRoot.gameObject.SetActive(true);
        for (int _btnIndex = 0; _btnIndex < BtnRoot.childCount; _btnIndex++)
        {

            if (_buttonSelf == BtnRoot.GetChild(_btnIndex).gameObject)
            {

                for (int _uiIndex = 0; _uiIndex < UIRoot.GetChild(0).childCount; _uiIndex++)
                {

                    UIRoot.GetChild(0).GetChild(_uiIndex).gameObject.SetActive(false);

                }
                UIRoot.GetChild(0).GetChild(_btnIndex).gameObject.SetActive(true);

                switch (_btnIndex)
                {

                    case 0://NewGame
                        Dict.SaveId = 0;
                        SceneManager.LoadScene(2);
                        break;
                    case 1://LoadGame
                        int _saveSlotsCount = Dict.GetColCount(Dict.SqlDBName, "z_save_base_info");
                        for (int _index = 0; _index < _saveSlotsCount; _index++)
                        {

                            //0: 未解锁 1: 已解锁但为空存档 2: 已解锁并有存储记录 3: 自动存档
                            int _value = Dict.GetInt(Dict.SqlDBName, "z_save_base_info", "value", _index + 1);

                            switch (_value)
                            {

                                case 0:
                                    SaveSlotRoot.GetChild(_index).GetChild(0).GetComponent<Text>().text = "[未解锁]";
                                    SaveSlotRoot.GetChild(_index).GetComponent<Image>().raycastTarget = false;
                                    SaveSlotRoot.GetChild(_index).GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f);
                                    break;
                                case 1:
                                    SaveSlotRoot.GetChild(_index).GetChild(0).GetComponent<Text>().text = "[空]";
                                    SaveSlotRoot.GetChild(_index).GetComponent<Image>().raycastTarget = false;
                                    SaveSlotRoot.GetChild(_index).GetComponent<Image>().color = Color.white;
                                    break;
                                case 2:
                                    SaveSlotRoot.GetChild(_index).GetChild(0).GetComponent<Text>().text = Dict.GetAllDict()["System"]["z_save_base_info"]["year"][_index + 1] + "/"
                                                                                                        + Dict.GetAllDict()["System"]["z_save_base_info"]["month"][_index + 1] + "/"
                                                                                                        + Dict.GetAllDict()["System"]["z_save_base_info"]["day"][_index + 1] + " "
                                                                                                        + Dict.GetAllDict()["System"]["z_save_base_info"]["hour"][_index + 1] + ":"
                                                                                                        + Dict.GetAllDict()["System"]["z_save_base_info"]["minute"][_index + 1] + ":"
                                                                                                        + Dict.GetAllDict()["System"]["z_save_base_info"]["second"][_index + 1];
                                    SaveSlotRoot.GetChild(_index).GetComponent<Image>().raycastTarget = true;
                                    SaveSlotRoot.GetChild(_index).GetComponent<Image>().color = Color.white;
                                    SaveSlotRoot.GetChild(_index).GetComponent<Button>().onClick.RemoveAllListeners();
                                    SaveSlotRoot.GetChild(_index).GetComponent<Button>().onClick.AddListener(Load);
                                    break;
                                case 3:
                                    SaveSlotRoot.GetChild(_index).GetChild(0).GetComponent<Text>().text = "[自动存档位]" + " - "
                                                                                                        + Dict.GetAllDict()["System"]["z_save_base_info"]["year"][_index + 1] + "/"
                                                                                                        + Dict.GetAllDict()["System"]["z_save_base_info"]["month"][_index + 1] + "/"
                                                                                                        + Dict.GetAllDict()["System"]["z_save_base_info"]["day"][_index + 1] + " "
                                                                                                        + Dict.GetAllDict()["System"]["z_save_base_info"]["hour"][_index + 1] + ":"
                                                                                                        + Dict.GetAllDict()["System"]["z_save_base_info"]["minute"][_index + 1] + ":"
                                                                                                        + Dict.GetAllDict()["System"]["z_save_base_info"]["second"][_index + 1];



                                    ;
                                    SaveSlotRoot.GetChild(_index).GetComponent<Button>().onClick.RemoveAllListeners();
                                    SaveSlotRoot.GetChild(_index).GetComponent<Button>().onClick.AddListener(Load);
                                    break;

                            }

                        }
                        break;
                    case 2://Config
                        Button _musicCtrBtn = UIRoot.GetChild(0).GetChild(_btnIndex).GetChild(1).GetChild(1).GetComponent<Button>();
                        Button _soundCtrBtn = UIRoot.GetChild(0).GetChild(_btnIndex).GetChild(1).GetChild(2).GetComponent<Button>();
                        _musicCtrBtn.onClick.RemoveAllListeners();
                        _soundCtrBtn.onClick.RemoveAllListeners();
                        _musicCtrBtn.onClick.AddListener(MyCommon.MusicSwitch);
                        _soundCtrBtn.onClick.AddListener(MyCommon.SoundSwitch);
                        break;
                    case 3://Intro
                        _introPageIndex = 0;
                        for (int _index = 0; _index < _introPages.Length; _index++)
                        {

                            if (_index == _introPageIndex)
                            {

                                _introPages[_index].gameObject.SetActive(true);

                            }
                            else
                            {

                                _introPages[_index].gameObject.SetActive(false);

                            }

                        }
                        _introBtns[0].gameObject.SetActive(false);
                        _introBtns[1].gameObject.SetActive(true);
                        break;
                    case 4://About

                        break;

                }

            }

        }

    }


    void Load()
    {

        GameObject _buttonSelf = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
        //id 0 为初始化用DB文件
        for (int _index = 0; _index < SaveSlotRoot.childCount; _index++)
        {

            if (_buttonSelf == SaveSlotRoot.GetChild(_index).gameObject)
            {

                Dict.SaveId = _index + 1;
                SceneManager.LoadScene(2);
                return;

            }

        }

    }


    void CloseTitleUI()
    {

        for (int _index = 0; _index < UIRoot.GetChild(0).childCount; _index++)
        {

            UIRoot.GetChild(0).GetChild(_index).gameObject.SetActive(false);

        }
        UIRoot.gameObject.SetActive(false);

    }


    /// <summary>
    /// Intro页中箭头Btn的功能
    /// </summary>
    void UpdateIntroPage()
    {

        GameObject _buttonSelf = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
        for (int _index = 0; _index < _introBtns.Length; _index++)
        {

            if (_buttonSelf == _introBtns[_index].gameObject)
            {

                switch (_index)
                {

                    case 0:
                        _introPageIndex--;
                        if (_introPageIndex < 0)
                        {

                            _introPageIndex = 0;

                        }
                        break;
                    case 1:
                        _introPageIndex++;
                        if (_introPageIndex > 2)
                        {

                            _introPageIndex = 2;

                        }
                        break;

                }

            }

        }

        for (int _index = 0; _index < _introPages.Length; _index++)
        {

            if (_index == _introPageIndex)
            {

                _introPages[_index].gameObject.SetActive(true);
                switch (_index)
                {

                    case 0:
                        _introBtns[0].gameObject.SetActive(false);
                        _introBtns[1].gameObject.SetActive(true);
                        break;
                    case 1:
                        _introBtns[0].gameObject.SetActive(true);
                        _introBtns[1].gameObject.SetActive(true);
                        break;
                    case 2:
                        _introBtns[0].gameObject.SetActive(true);
                        _introBtns[1].gameObject.SetActive(false);
                        break;

                }

            }
            else
            {

                _introPages[_index].gameObject.SetActive(false);

            }

        }

    }

}
