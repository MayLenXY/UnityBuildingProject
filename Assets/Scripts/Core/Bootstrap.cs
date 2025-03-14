using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Core
{
    public class Bootstrap : MonoBehaviour
    {
        private void Awake()
        {
            GameInitializer.Initialize();
        }
    }

    public static class GameInitializer
    {
        public static void Initialize()
        {
            Debug.Log("Game Initialized");
            InitializeSystems();
        }

        private static void InitializeSystems()
        {
            Debug.Log("Initializing game systens...");
        }
    }
}
