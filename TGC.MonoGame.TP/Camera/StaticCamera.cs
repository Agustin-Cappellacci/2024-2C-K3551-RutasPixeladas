using Microsoft.Xna.Framework;

namespace TGC.MonoGame.TP.Content.Models
{
    /// <summary>
    ///     Static camera without restrictions, where each component is configured and nothing is inferred.
    /// </summary>
    public class StaticCamera
    {
        public float AspectRatio { get; set; }

        public float FarPlane { get; set; }

        public float FieldOfView { get; set; }

        public float NearPlane { get; set; }

        public Vector3 FrontDirection { get; set; }

        public Matrix Projection { get; set; }

        public Vector3 Position { get; set; }

        public Vector3 RightDirection { get; set; }

        public Vector3 UpDirection { get; set; }

        public Matrix View { get; set; }
        /// <summary>
        ///     Static camera looking at a particular direction, which has the up vector (0,1,0).
        /// </summary>
        /// <param name="aspectRatio">Aspect ratio, defined as view space width divided by height.</param>
        /// <param name="position">The position of the camera.</param>
        /// <param name="frontDirection">The direction where the camera is pointing.</param>
        /// <param name="upDirection">The direction that is "up" from the camera's point of view.</param>
        public StaticCamera(float aspectRatio, Vector3 position, Vector3 frontDirection, Vector3 upDirection)
        {
            Position = position;
            FrontDirection = frontDirection;
            UpDirection = upDirection;
            BuildView();
        }

        /// <summary>
        ///     Build the camera View matrix using its properties.
        /// </summary>
        public void BuildView()
        {
            View = Matrix.CreateLookAt(Position, Position + FrontDirection, UpDirection);
        }

        public void BuildProjection(float aspectRatio, float nearPlaneDistance, float farPlaneDistance,
    float fieldOfViewDegrees)
        {
            Projection = Matrix.CreatePerspectiveFieldOfView(fieldOfViewDegrees, aspectRatio, nearPlaneDistance,
                farPlaneDistance);
        }
        /// <inheritdoc />
        public void Update(GameTime gameTime)
        {
            // This camera has no movement, once initialized with position and lookAt it is no longer updated automatically.
        }
    }
}