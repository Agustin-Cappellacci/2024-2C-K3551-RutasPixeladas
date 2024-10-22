using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TGC.MonoGame.TP.Content.Models
{
    class FreeCamera
    {
        public Matrix Projection { get; private set; }
        public Matrix View { get; private set; }
        private Matrix Scale { get; set; }
        private float Rotation { get; set; }
        private float Yaw { get; set; }
        private float Pitch { get; set; }

        private Vector3 CameraPosition;
        private Vector3 CameraForward;
        private Vector3 CameraTarget;
        private Vector3 CameraUp;

        public FreeCamera(float aspectRatio)
        {
            // Orthographic camera
            // Projection = Matrix.CreateOrthographic(screenWidth, screenHeight, 0.01f, 10000f);

            // Perspective camera
            // Uso 60° como FOV, aspect ratio, pongo las distancias a near plane y far plane en 0.1 y 100000 (mucho) respectivamente

            CameraPosition = new Vector3(-600, 300, 300);
            CameraForward = Vector3.Forward;
            CameraTarget = Vector3.Zero;
            CameraUp = Vector3.Up;

            Scale = Matrix.CreateScale(1f);
            View = Matrix.CreateLookAt(CameraPosition, CameraTarget, CameraUp);
            Projection = Matrix.CreatePerspectiveFieldOfView(MathF.PI / 3f, aspectRatio, 0.1f, 100000f);
        }

        public void Update(GameTime gameTime, Matrix followedWorld)
        {

            var keyboardState = Keyboard.GetState();
            var mouseState = Mouse.GetState();
            var cameraSpeed = 500f;
            var rotationSpeed = 0.02f;

            // --- Captura de la rotación con las teclas ---
            if (keyboardState.IsKeyDown(Keys.J))
                Yaw -= rotationSpeed;  // Rotar hacia la izquierda
            if (keyboardState.IsKeyDown(Keys.L))
                Yaw += rotationSpeed;  // Rotar hacia la derecha
            if (keyboardState.IsKeyDown(Keys.I))
                Pitch -= rotationSpeed; // Mirar hacia arriba
            if (keyboardState.IsKeyDown(Keys.K))
                Pitch += rotationSpeed; // Mirar hacia abajo

            // Limitar la rotación vertical para evitar que la cámara se dé vuelta
            Pitch = MathHelper.Clamp(Pitch, -MathHelper.PiOver2 + 0.1f, MathHelper.PiOver2 - 0.1f);


            // Calcular la dirección hacia adelante (forward) a partir del yaw y pitch
            CameraForward = Vector3.Normalize(new Vector3(
                (float)(Math.Cos(Pitch) * Math.Cos(Yaw)),
                (float)Math.Sin(Pitch),
                (float)(Math.Cos(Pitch) * Math.Sin(Yaw))
            ));

            // También podemos calcular la dirección hacia la derecha (para el movimiento lateral)
            Vector3 CameraRight = Vector3.Cross(CameraForward, CameraUp);


            // Input de teclado para mover la cámara

            // --- Captura del movimiento con WASD ---
            if (keyboardState.IsKeyDown(Keys.W))
                if (keyboardState.IsKeyDown(Keys.LeftShift))
                {
                    CameraPosition += CameraForward * 10 * cameraSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                }
                else
                    CameraPosition += CameraForward * cameraSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (keyboardState.IsKeyDown(Keys.S))
                if (keyboardState.IsKeyDown(Keys.LeftShift))
                {
                    CameraPosition -= CameraForward * 10 * cameraSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                }
                else
                    CameraPosition -= CameraForward * cameraSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (keyboardState.IsKeyDown(Keys.A))
                if (keyboardState.IsKeyDown(Keys.LeftShift))
                {
                    CameraPosition -= CameraRight * 10 * cameraSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                }
                else
                    CameraPosition -= CameraRight * cameraSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (keyboardState.IsKeyDown(Keys.D))
                if (keyboardState.IsKeyDown(Keys.LeftShift))
                {
                    CameraPosition += CameraRight * 10 * cameraSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                }
                else
                    CameraPosition += CameraRight * cameraSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Basado en el tiempo que paso se va generando una rotacion.
            Rotation += Convert.ToSingle(gameTime.ElapsedGameTime.TotalSeconds);

            CameraTarget = CameraPosition + CameraForward;

            View = Matrix.CreateLookAt(CameraPosition, CameraTarget, CameraUp);
        }
    }
}
