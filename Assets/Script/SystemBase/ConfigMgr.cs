using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ConfigMgr : SingleToneBase<ConfigMgr>
{
    public abstract class ConfigInfo<T> where T : IComparable
    {
        public string Key { get; protected set; }
        public T DefaultValue;
        protected T v;
        bool Inited;
        public T Value
        {
            get
            {
                if (!Inited)
                {
                    Get();
                    Inited = true;
                }

                return v;
            }

            set
            {
                if (!v.Equals(value))
                {
                    v = value;
                    Set();
                    if (OnChange != null)
                    {
                        OnChange(v);
                    }
                }
            }
        }
        protected abstract void Set();
        protected abstract void Get();
        protected Action<T> OnChange = null;
        public ConfigInfo(string key, T defaultValue)
        {
            this.Key = key;
            this.DefaultValue = defaultValue;
        }
        public void Delete()
        {
            if (PlayerPrefs.HasKey(Key))
            {
                PlayerPrefs.DeleteKey(Key);
            }

            this.Inited = false;
        }
    }

    //==============以下詳細ConfigInfoのクラス===============//
    public class IntConfig : ConfigInfo<int>
    {
        public IntConfig(string key, int defaultValue) : base(key, defaultValue) { }
        protected override void Set()
        {
            PlayerPrefs.SetInt(Key, v);
        }
        protected override void Get()
        {
            v = PlayerPrefs.GetInt(Key, DefaultValue);
        }
    }

    public class BoolConfig : ConfigInfo<bool>
    {
        public BoolConfig(string key, bool defaultValue) : base(key, defaultValue) { }
        protected override void Set()
        {
            PlayerPrefs.SetInt(Key, ConverBoolToInt(v));
        }
        protected override void Get()
        {
            v = ConverBoolToInt(PlayerPrefs.GetInt(Key, ConverBoolToInt(DefaultValue)));
        }
        int ConverBoolToInt(bool b)
        {
            return b ? 1 : 0;
        }
        bool ConverBoolToInt(int i)
        {
            return i != 0;
        }
    }
    public class StirngConfig : ConfigInfo<string>
    {
        public StirngConfig(string key, string defaultValue) : base(key, defaultValue) { }
        protected override void Set()
        {
            PlayerPrefs.SetString(Key, v);
        }
        protected override void Get()
        {
            v = PlayerPrefs.GetString(Key, DefaultValue);
        }
    }


    //===================================================//
    //欲しいConfigをこの中に入れる
    public class GameConfigCtrl
    {
        public StirngConfig playerName = new StirngConfig("Config.PlayerName", "PlayerAAAA");
    }
    public GameConfigCtrl gameConfigCtrl = new GameConfigCtrl();
    //取得するときに、これ使う
    static public ConfigMgr.GameConfigCtrl GetGameConfig()
    {
        return Get().gameConfigCtrl;
    }
}
