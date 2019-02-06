using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class Common : MonoBehaviour
{

    List<AudioClip> MusicClips { get; set; }
    public List<AudioSource> MusicSource { get; set; }


    // Use this for initialization
    void Awake()
    {

        DontDestroyOnLoad(gameObject);

        //字典初始化
        Dict.SaveId = 0;
        Dict.SqlDBName = "Sql";
        Dict.WallDBName = "Wall";
        Dict.SystemDBName = "System";
        Dict.InitDict();

        //----------------------------------------------

        //音效
        MusicClips = new List<AudioClip>
        {

            Resources.Load<AudioClip>("Sounds/Attack1"),    //0 攻击
            Resources.Load<AudioClip>("Sounds/GetItem1"),   //1 获得普通物品
            Resources.Load<AudioClip>("Sounds/GetItem2"),   //2 获得钥匙
            Resources.Load<AudioClip>("Sounds/OpenDoor1"),  //3 普通门
            Resources.Load<AudioClip>("Sounds/OpenDoor2"),  //4 机关门
            Resources.Load<AudioClip>("Sounds/Read"),       //5 消息
            Resources.Load<AudioClip>("Sounds/Walk"),       //6 行走
            Resources.Load<AudioClip>("Sounds/Music_Title"),//7 主界面场景
            Resources.Load<AudioClip>("Sounds/Music_Game"), //8 游戏场景
            Resources.Load<AudioClip>("Sounds/Music_End"),  //9 游戏结局场景
            Resources.Load<AudioClip>("Sounds/Attack2")   //10 魔法攻击

        };

        MusicSource = new List<AudioSource>();
        for (int i = 0; i < MusicClips.Count; i++)
        {

            MusicSource.Add(gameObject.AddComponent<AudioSource>());
            MusicSource[i].clip = MusicClips[i];
            if (i == 7 || i == 8 || i == 9)
            {

                MusicSource[i].loop = true;

            }

        }

        //MusicSource[7].Play();
        //----------------------------------------------
        SceneManager.LoadScene(1);

    }


    public void MusicSwitch()
    {

        string _col = "is_music_on";
        int _value = Dict.GetInt(Dict.SqlDBName, "system", _col, 1);
        Image _musicBtnImg = null;
        switch (SceneManager.GetActiveScene().buildIndex)
        {

            case 1:
                _musicBtnImg = FindObjectOfType<TitleManager>().transform.GetChild(1).GetChild(0).GetChild(2).GetChild(1).GetChild(1).GetComponent<Image>();
                break;
            case 2:
                _musicBtnImg = GameObject.FindWithTag("RootUI").transform.GetChild(3).GetChild(0).GetChild(1).GetChild(0).GetChild(1).GetChild(1).GetChild(1).GetComponent<Image>();
                break;

        }

        //当前为打开
        if (_value == 0)
        {

            Dict.SetInt(Dict.SqlDBName, "system", _col, 1, 1);
            _musicBtnImg.color = new Color(1, 1, 1, 0);
            foreach (AudioSource _s in MusicSource)
            {

                if (_s == MusicSource[7] || _s == MusicSource[8] || _s == MusicSource[9])
                {

                    _s.mute = true;

                }

            }

        }
        //当前为关闭
        else
        {

            Dict.SetInt(Dict.SqlDBName, "system", _col, 1, 0);
            _musicBtnImg.color = new Color(1, 1, 1, 1);
            foreach (AudioSource _s in MusicSource)
            {

                if (_s == MusicSource[7] || _s == MusicSource[8] || _s == MusicSource[9])
                {

                    _s.mute = false;

                }

            }

        }

    }
    public void SoundSwitch()
    {

        string _col = "is_sound_on";
        int _value = Dict.GetInt(Dict.SqlDBName, "system", _col, 1);
        Image _soundBtnImg = null;
        switch (SceneManager.GetActiveScene().buildIndex)
        {

            case 1:
                _soundBtnImg = FindObjectOfType<TitleManager>().transform.GetChild(1).GetChild(0).GetChild(2).GetChild(1).GetChild(2).GetComponent<Image>();
                break;
            case 2:
                _soundBtnImg = GameObject.FindWithTag("RootUI").transform.GetChild(3).GetChild(0).GetChild(1).GetChild(0).GetChild(1).GetChild(1).GetChild(2).GetComponent<Image>();
                break;

        }
        //当前为打开
        if (_value == 0)
        {

            Dict.SetInt(Dict.SqlDBName, "system", _col, 1, 1);
            _soundBtnImg.color = new Color(1, 1, 1, 0);
            foreach (AudioSource _s in MusicSource)
            {

                if (_s == MusicSource[7] || _s == MusicSource[8] || _s == MusicSource[9])
                {

                    continue;

                }
                _s.mute = true;

            }

        }
        //当前为关闭
        else
        {

            Dict.SetInt(Dict.SqlDBName, "system", _col, 1, 0);
            _soundBtnImg.color = new Color(1, 1, 1, 1);
            foreach (AudioSource _s in MusicSource)
            {

                if (_s == MusicSource[7] || _s == MusicSource[8] || _s == MusicSource[9])
                {

                    continue;

                }
                _s.mute = false;

            }

        }

    }


    /// <summary>
    /// Plaies the bgm.
    /// </summary>
    /// <param name="_musicid">Musicid.  id为 7 8 9</param>
    public void PlayBGM(int _musicid)
    {

        MusicSource[7].Stop();
        MusicSource[8].Stop();
        MusicSource[9].Stop();
        MusicSource[_musicid].Play();

    }

}
