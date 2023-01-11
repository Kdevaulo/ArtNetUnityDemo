using System;
using System.Collections.Generic;

using UnityEngine;

namespace UnityArtNetDemo.StripExtender
{
    [Serializable]
    public class GameObjectCollection
    {
        public List<GameObject> GameObjects => _gameObjects;

        [SerializeField] private List<GameObject> _gameObjects = new List<GameObject>();
    }
}