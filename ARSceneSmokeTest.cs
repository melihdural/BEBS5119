// Assets/Tests/PlayMode/ARSceneSmokeTest.cs

using System.Collections;
using NUnit.Framework;
using Presentation;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.XR.ARFoundation;

namespace Tests.PlayMode
{
    /// <summary>
    /// AR sahnesi için temel smoke test
    /// </summary>
    public class ARSceneSmokeTest
    {
        [UnityTest]
        public IEnumerator MainARScene_LoadsSuccessfully()
        {
            // Arrange & Act
            SceneManager.LoadScene("MainAR");
            yield return null;

            // Assert
            Assert.IsTrue(SceneManager.GetActiveScene().name == "MainAR");
        }

        [UnityTest]
        public IEnumerator ARSession_Exists()
        {
            // Arrange
            SceneManager.LoadScene("MainAR");
            yield return null;

            // Act
            var arSession = Object.FindObjectOfType<ARSession>();

            // Assert
            Assert.IsNotNull(arSession, "AR Session should exist in scene");
        }

        [UnityTest]
        public IEnumerator ARPlaneManager_Exists()
        {
            // Arrange
            SceneManager.LoadScene("MainAR");
            yield return null;

            // Act
            var planeManager = Object.FindObjectOfType<ARPlaneManager>();

            // Assert
            Assert.IsNotNull(planeManager, "AR Plane Manager should exist");
        }

        [UnityTest]
        public IEnumerator HUDController_Exists()
        {
            // Arrange
            SceneManager.LoadScene("MainAR");
            yield return null;

            // Act
            var hud = Object.FindObjectOfType<HUDController>();

            // Assert
            Assert.IsNotNull(hud, "HUD Controller should exist");
        }

        [UnityTest]
        public IEnumerator ARAnchorManager_Exists()
        {
            // Arrange
            SceneManager.LoadScene("MainAR");
            yield return null;

            // Act
            var anchorManager = Object.FindObjectOfType<ARAnchorManager>();

            // Assert
            Assert.IsNotNull(anchorManager, "AR Anchor Manager should exist");
        }
    }
}