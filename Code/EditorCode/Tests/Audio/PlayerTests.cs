﻿using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using UnityTK.Audio;

namespace UnityTK.Test.Audio
{
	public class PlayerTests
	{

		[Test]
		public void TestOneShotPlayer()
		{
			// Arrange
			var wsPrefab = new GameObject("WSPrefab");
			var proxPrefab = new GameObject("ProxPrefab");
			var nsPrefab = new GameObject("NSPrefab");
			GameObject testGo = new GameObject("TEST");

			var wsPrefabAS = wsPrefab.AddComponent<UTKAudioSource>();
			var proxPrefabAS = proxPrefab.AddComponent<ProximityBasedAudio>();
			var nsPrefabAS = nsPrefab.AddComponent<NonSpatialAudioSource>();

			var audioOneShotPlayer = new GameObject("OSPlayer");
			var aosp = audioOneShotPlayer.AddComponent<AudioOneShotPlayer>();
			aosp.Awake();
			AudioOneShotPlayer.instance.worldspacePrefab = wsPrefabAS;
			AudioOneShotPlayer.instance.proximityBasedPrefab = proxPrefabAS;
			AudioOneShotPlayer.instance.nonSpatialPrefab = nsPrefabAS;

			var proximityPlayer = new GameObject("Proximity Player");
			var pp = proximityPlayer.AddComponent<ProximityPlayer>();
			pp.Awake();

			var cameraGo = new GameObject("Camera");
			pp.proximityCamera = cameraGo.AddComponent<Camera>();

			AudioEventMock evtMock = new AudioEventMock();
			bool wasPlayed = false;
			IUTKAudioSource playedSource = null;
			bool wasLooped = false;
			evtMock.onPlay += (source, loop) =>
			{
				wasLooped = loop;
				wasPlayed = true;
				playedSource = source;
			};

			// Act
			AudioOneShotPlayer.instance.PlayNonSpatial(evtMock);

			Assert.IsTrue(wasPlayed);
			Assert.IsFalse(wasLooped);
			Assert.AreNotEqual(null, playedSource);
			Assert.AreEqual(typeof(NonSpatialAudioSource), playedSource.GetType());

			// Reset and act again
			wasPlayed = wasLooped = false;
			playedSource = null;
			AudioOneShotPlayer.instance.PlayWorldspace(evtMock, testGo);

			Assert.IsTrue(wasPlayed);
			Assert.IsFalse(wasLooped);
			Assert.AreNotEqual(null, playedSource);
			Assert.AreEqual(testGo.transform, ((UTKAudioSource)playedSource).transform.parent);
			Assert.AreEqual(typeof(UTKAudioSource), playedSource.GetType());

			// Reset and act again
			wasPlayed = wasLooped = false;
			playedSource = null;
			AudioOneShotPlayer.instance.PlayProximity(evtMock, testGo);

			Assert.IsTrue(wasPlayed);
			Assert.IsFalse(wasLooped);
			Assert.AreNotEqual(null, playedSource);
			Assert.AreEqual(testGo.transform, ((ProximityBasedAudio)playedSource).transform.parent);
			Assert.AreEqual(typeof(ProximityBasedAudio), playedSource.GetType());

			// Reset and act again
			wasPlayed = wasLooped = false;
			playedSource = null;
		}

	}
}