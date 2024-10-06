﻿//
// Copyright 2017-2023 Valve Corporation.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//

using System;
using UnityEngine;

namespace SteamAudio
{
    [AddComponentMenu("Steam Audio/Steam Audio Dynamic Object")]
    public class SteamAudioDynamicObject : MonoBehaviour
    {
        [Header("Export Settings")]
        public SerializedData asset = null;

#if STEAMAUDIO_ENABLED
        InstancedMesh mInstancedMesh = null;

        private void OnDestroy()
        {
            SteamAudioManager.UnloadDynamicObject(this);

            if (mInstancedMesh != null)
            {
                mInstancedMesh.Release();
            }
        }

        private void OnEnable()
        {
            if (mInstancedMesh != null)
            {
                mInstancedMesh.AddToScene(SteamAudioManager.CurrentScene);
                SteamAudioManager.ScheduleCommitScene();
            }
        }

        private void OnDisable()
        {
            if (mInstancedMesh != null && SteamAudioManager.CurrentScene != null)
            {
                mInstancedMesh.RemoveFromScene(SteamAudioManager.CurrentScene);
                SteamAudioManager.ScheduleCommitScene();
            }
        }

        private void Update()
        {
            if (mInstancedMesh == null && asset != null)
            {
                mInstancedMesh = SteamAudioManager.LoadDynamicObject(this, SteamAudioManager.CurrentScene, SteamAudioManager.Context);

                if (enabled)
                {
                    mInstancedMesh.AddToScene(SteamAudioManager.CurrentScene);
                    SteamAudioManager.ScheduleCommitScene();
                }
            }

            // Only update the dynamic object if it has actually move this frame
            if (transform.hasChanged)
            {
                mInstancedMesh.UpdateTransform(SteamAudioManager.CurrentScene, transform);
                SteamAudioManager.ScheduleCommitScene();
                transform.hasChanged = false;
            }
        }
#endif
    }
}
