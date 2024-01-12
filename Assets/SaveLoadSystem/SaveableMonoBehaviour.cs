using SaveLoadSystem.Core;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace SaveLoadSystem
{

    public class SaveableMonoBehaviour : MonoBehaviour, ISaveable
    {


        [SerializeField] private string _id;

        [SerializeField] private bool _savePosition;
        [SerializeField] private bool _saveRotation;
        [SerializeField] private bool _saveScale;


        protected virtual void Awake()
        {
            SaveableData savedData = SaveLoadManager.Load(_id);
            if (savedData != null)
                LoadSavedData(savedData);
        }


        protected virtual void OnDestroy()
        {
            SaveLoadManager.Save(this,_id);
        }


        public SaveableData ConvertToSaveableData()
        {
            SaveableData saveData = new SaveableData();

            if (_savePosition)
            {
                saveData.Write("position", transform.position);
            }
            if(_saveRotation)
            {
                saveData.Write("rotation", transform.rotation);
            }
            if (_saveScale)
            {
                saveData.Write("scale", transform.localScale);
            }


            foreach (FieldInfo field in this.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public))
            {
                if (field.IsDefined(typeof(SaveFieldAttribute), true))
                {
                    var value = field.GetValue(this);
                    saveData.Write(field.Name, value);
                }
            }

            return saveData;
        }



        public void LoadSavedData(SaveableData savedData)
        {
            if (_savePosition && savedData.TryRead<Vector3>("position", out Vector3 pos))
            {
                transform.position = pos;
            }
            if (_saveRotation && savedData.TryRead<Quaternion>("rotation", out Quaternion rot))
            {
                transform.rotation = rot;
            }
            if (_saveScale && savedData.TryRead<Vector3>("sacale", out Vector3 sca))
            {
                transform.localScale = sca;
            }

            foreach (FieldInfo field in this.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public))
            {
                if (field.IsDefined(typeof(SaveFieldAttribute), true))
                {
                    if (savedData.TryRead(field.Name, out var loadedValue))
                    {
                        field.SetValue(this, loadedValue);
                    }
                }
            }

        }



    }




}