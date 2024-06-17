using HarmonyLib;
using System.Reflection;
using CWissKnife;
using UnityEngine;
using System.Collections.Generic;
using System;
using Photon.Pun;

[HarmonyPatch(typeof(PlayerController), "Update")]
public class NoClipPatch {
    private static bool wereCollisionsFixed = true;
    private static List<Collider> disabledColliders = new();
    private static bool jumpPressed = false;
    private static bool crouchPressed = false;
    private static BodypartType bodypart;
    static void Prefix(PlayerRagdoll ___ragdoll, Player ___player) {
        if (!Plugin.configToggleNoclip.Value || !PhotonNetwork.IsMasterClient || MainMenuHandler.SteamLobbyHandler.IsPlayingWithRandoms()) {
            if (!wereCollisionsFixed) {
                foreach (Collider disabledCollider in disabledColliders) {
                    if (disabledCollider != null) {
                        disabledCollider.isTrigger = false;
                    }
                }
                wereCollisionsFixed = true;
                ___player.data.sinceGrounded = .1f;
                disabledColliders.Clear();
            }
            return;
        }
        if (!___player.data.isLocal) {
            return;
        }

        Collider[] componentsInChildren = ___ragdoll.GetComponentsInChildren<Collider>();
        for (int i = 0; i < componentsInChildren.Length; i++) {
            if (!componentsInChildren[i].isTrigger || !disabledColliders.Contains(componentsInChildren[i])) {
                // Only add first routine to list
                if (Enum.TryParse<BodypartType>(componentsInChildren[i].transform.parent.name, out bodypart)) {
                    disabledColliders.Add(componentsInChildren[i]);
                }
                componentsInChildren[i].isTrigger = true;
            }
        }
        wereCollisionsFixed = false;

        jumpPressed = ___player.input.jumpIsPressed;
        crouchPressed = ___player.input.crouchIsPressed;
        ___player.input.jumpWasPressed = false;
        ___player.input.jumpIsPressed = false;
    }

    static void Postfix(PlayerRagdoll ___ragdoll, Player ___player) {
        if (!PhotonNetwork.IsMasterClient || MainMenuHandler.SteamLobbyHandler.IsPlayingWithRandoms()) {
            return;
        }

        if (!Plugin.configToggleNoclip.Value || !___player.data.isLocal)
            return;
        if (jumpPressed) {
            jumpPressed = false;
            float verticalSpeed = Plugin.configToggleSpeedMultiplier.Value ? Plugin.configSpeedMultiplier.Value : 1f;
            var AddOpposingForce = typeof(PlayerRagdoll).GetMethod("AddForce", BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { typeof(Vector3), typeof(ForceMode) }, null);
            AddOpposingForce.Invoke(___ragdoll, new object[] { Vector3.up * verticalSpeed * 7f, ForceMode.Acceleration });
        }
        if (crouchPressed) {
            crouchPressed = false;
            float verticalSpeed = Plugin.configToggleSpeedMultiplier.Value ? Plugin.configSpeedMultiplier.Value : 1f;
            var AddOpposingForce = typeof(PlayerRagdoll).GetMethod("AddForce", BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { typeof(Vector3), typeof(ForceMode) }, null);
            AddOpposingForce.Invoke(___ragdoll, new object[] { Vector3.down * verticalSpeed * 7f, ForceMode.Acceleration });
        }
    }
}

[HarmonyPatch(typeof(PlayerController), "Gravity")]
public class NoClipPatch2 {
    static bool Prefix(Player ___player) {
        if (!PhotonNetwork.IsMasterClient || MainMenuHandler.SteamLobbyHandler.IsPlayingWithRandoms()) {
            return true;
        }

        if (!Plugin.configToggleNoclip.Value || !___player.data.isLocal)
            return true;
        return false;
    }
}

[HarmonyPatch(typeof(PlayerController), "ConstantGravity")]
public class NoClipPatch3 {
    static bool Prefix(Player ___player) {
        if (!PhotonNetwork.IsMasterClient || MainMenuHandler.SteamLobbyHandler.IsPlayingWithRandoms()) {
            return true;
        }

        if (!Plugin.configToggleNoclip.Value || !___player.data.isLocal)
            return true;
        return false;
    }
}

[HarmonyPatch(typeof(PlayerController), "MovementStateChanges")]
public class NoClipPatch4 {
    static void Postfix(Player ___player) {
        if (!PhotonNetwork.IsMasterClient || MainMenuHandler.SteamLobbyHandler.IsPlayingWithRandoms()) {
            return;
        }

        if (!Plugin.configToggleNoclip.Value || !___player.data.isLocal)
            return;
        ___player.data.isCrouching = false;
    }
}

[HarmonyPatch(typeof(PlayerController), "TryJump")]
public class NoClipPatch5 {
    static bool Prefix(Player ___player) {
        if (!PhotonNetwork.IsMasterClient || MainMenuHandler.SteamLobbyHandler.IsPlayingWithRandoms()) {
            return true;
        }

        if (!Plugin.configToggleNoclip.Value || !___player.data.isLocal)
            return true;
        return false;
    }
}


[HarmonyPatch(typeof(PlayerRagdoll), "BodyChanged")]
public class NoClipPatch6 {
    static bool Prefix(Player ___player, List<Rigidbody> ___rigList) {
        if (!PhotonNetwork.IsMasterClient || MainMenuHandler.SteamLobbyHandler.IsPlayingWithRandoms()) {
            return true;
        }

        if (!Plugin.configToggleNoclip.Value || !___player.data.isLocal)
            return true;
		for (int i = 0; i < ___rigList.Count; i++)
		{
            ___rigList[i].mass = 10f;
		}
		___player.data.totalMass = 190f;
        return false;
    }
}

[HarmonyPatch(typeof(DiveBellPlayerDetector), nameof(DiveBellPlayerDetector.CheckForPlayers))]
// Diving bell missing players fix (https://github.com/FlawTECH/CWissKnife/issues/2)
public class NoClipPatch7 {
    static void Postfix(ref ICollection<Player> __result, DiveBellPlayerDetector __instance, Collider[] ___results) {
        if (!PhotonNetwork.IsMasterClient || MainMenuHandler.SteamLobbyHandler.IsPlayingWithRandoms()) {
            return;
        }

        if (!Plugin.configToggleNoclip.Value)
            return;

        foreach (Transform transform in __instance.m_detectors)
		{
			int num = Physics.OverlapBoxNonAlloc(transform.position, transform.lossyScale * 0.5f, ___results, transform.rotation);
			for (int j = 0; j < num; j++)
			{
				Collider collider = ___results[j];
                Player componentInParent = collider.GetComponentInParent<Player>();
                if (componentInParent && !__result.Contains(componentInParent)) {
                    __result.Add(componentInParent);
                }
            }
        }
    }
}