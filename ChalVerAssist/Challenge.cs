﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace ChalVerAssist
{
    public class Challenge
    {
        public Challenge(ChallengeData data, RainWorldGame game)
        {
            //ChalVerAssist.Logger.LogMessage("Challenge created: " + name);
            this.data = data;
            this.game = game;
            camera = game.cameras[0];
            ActiveChallenge = this;
            SetAllowed();
            if (data.HasTimer)
                timer = new ChallengeTimer(game, this);
        }

        public static Challenge ActiveChallenge;
        public static ChallengeData SelectedChallenge;

        public RainWorldGame game;
        public Player player;
        private bool available;
        private bool _forcedUnavailability;
        public bool allowed { get; private set; }
        public ChallengeData data;
        public ChallengeTimer timer;
        public bool Available
        {
            get { return available; }
            set { _forcedUnavailability = !value; } // Made to be able to force a challenge to fail with extra code.
        }
        private string playerRoom;
        public string PlayerRoom
        {
            get { return playerRoom; }
            set { 
                if (playerRoom != value)
                {
                    playerRoom = value;
                    dirtyRoom = true;
                }
            }
        }
        private RoomCamera camera;
        private HashSet<string> visitedRooms = null;
        private int roomIndex = 0;
        private bool dirtyRoom = false;
        private bool hasVisitedRooms = false;
        public bool MetEcho = false; // Handled with a hook in Ghost.StartConversation()

        public void SetAllowed()
        {
            allowed = false;
            if (!game.IsStorySession)
                return;
            if (data.AllowedSlugcats != null && !data.AllowedSlugcats.Contains(game.StoryCharacter))
                return;
            if (data.AllowedRegions != null && !data.AllowedRegions.Contains(game.world.region.name))
                return;
            if (data.FirstCycle && game.GetStorySession.saveState.cycleNumber != 0)
                return;
            if (data.MeetEcho)
            {
                try
                {
                    var echo = GhostWorldPresence.GetGhostID(data.Rooms.Last().Split('_')[0]);
                    var dpsd = game.GetStorySession.saveState.deathPersistentSaveData;
                    int encounters = 0;
                    if (dpsd.ghostsTalkedTo.ContainsKey(echo))
                    {
                        encounters = dpsd.ghostsTalkedTo[echo];
                    }
                    if (!GhostWorldPresence.SpawnGhost(echo, dpsd.karma, dpsd.karmaCap, encounters, game.StoryCharacter == SlugcatStats.Name.Red))
                        return;
                }
                catch (Exception ex)
                {
                    ChalVerAssist.Logger.LogError(ex);
                }
            }
            allowed = true;
        }

        public void Update()
        {
            if (!allowed)
                return;
            if (Input.GetKey(ChalVerOptionInterface.cfgUTurnTimerReset.Value))
            {
                ResetRecords();
            }
            // Slugcat must exist for the challenge to be updated
            if (player == null)
            {
                if (game.Players.Count > 0 && game.Players[0].realizedCreature != null)
                    player = game.Players[0].realizedCreature as Player;
                else
                    return;
            }

            // Useful for a variety of cases, camera's room is used because that has been ruled for timing RTA challenges
            PlayerRoom = camera.room.abstractRoom.name;

            // Timer can still change if the challenge isn't available, such as fading out, so it should be updated too.
            if (data.HasTimer)
                timer.Update();
            
            // Start challenge
            if (!available)
            {
                InitAvailability();
            }

            // See if the challenge should fail; stop execution if it's not even in progress
            if (available)
            {
                TryDisallow();
                if (!available)
                {
                    Fail();
                    return;
                }
            }
            else
                return;
            
            // Room crossing win condition
            if (data.Rooms != null && !hasVisitedRooms && dirtyRoom && !RoomSpecificConditions())
            {
                dirtyRoom = false;
                if (data.PathType == ChallengeData.PathTypeID.Unordered)
                {
                    if (data.Rooms.Contains(playerRoom) && !visitedRooms.Contains(playerRoom))
                        visitedRooms.Add(playerRoom);
                    if (visitedRooms.Count == data.Rooms.Length)
                        hasVisitedRooms = true;
                }
                else
                {
                    if (playerRoom == data.Rooms[roomIndex])
                        roomIndex++;
                    if (roomIndex == data.Rooms.Length)
                        hasVisitedRooms = true;
                }
            }

            TryComplete();
        }

        // InitAvailability should always set it to false if TryDisallow does too, can't have the challenge starting and failing on every tick
        private void InitAvailability()
        {
            // 
            if (_forcedUnavailability)
                return;
            if (data.Rooms != null)
            {
                if (data.PathType == ChallengeData.PathTypeID.Unordered && !data.Rooms.Contains(playerRoom))
                    return;
                else if (data.PathType != ChallengeData.PathTypeID.Unordered)
                {
                    if (data.Rooms[0] != playerRoom)
                        return;
                    if (data.StartNode != -1 && player.abstractCreature.pos.abstractNode != data.StartNode)
                        return;
                }
            }
            if (data.AvoidRooms != null && data.AvoidRooms.Contains(playerRoom))
                return;
            available = true;
        }
        private void TryDisallow()
        {
            available = false;
            if (data.AvoidRooms != null && data.AvoidRooms.Contains(playerRoom))
                return;
            if (data.Rooms != null && data.PathType == ChallengeData.PathTypeID.Strict && !data.Rooms.Contains(playerRoom))
                return;
            OnActivate();
            available = true;
        }

        public bool RoomSpecificConditions()
        {
            // abstractRoom.gate is preferred over IsGateRoom because fake gates can't be crossed
            if (data.CrossGates && camera.room.abstractRoom.gate && camera.room.regionGate?.mode != RegionGate.Mode.ClosingMiddle)
                return false;

            return true;
        }

        public void TryComplete()
        {
            if (data.Rooms != null && !hasVisitedRooms)
                return;
            if (data.MeetEcho && !MetEcho)
                return;
            if (data.HasTimer)
            {
                timer.Finish();
                if (data.TargetTime != -1 && timer.display.PassedTime > data.TargetTime)
                {
                    Fail();
                    return;
                }
            }
        }

        public void OnActivate()
        {
            MetEcho = false;
            roomIndex = 0;
        }
        public void Fail()
        {
            hasVisitedRooms = false;
            if (data.HasTimer)
                timer.ResetTimer();
        }

        public void ResetRecords()
        {
            Fail();
            ChallengeMSD.WipeRecord(data.Key);
        }
    }
}
