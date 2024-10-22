# Recordar

Para dibujar le modelo necesitamos pasarle informacion que el efecto esta esperando. En el método Draw.

````C#
Effect.Parameters["View"].SetValue(View);
Effect.Parameters["Projection"].SetValue(Projection);
Effect.Parameters["DiffuseColor"].SetValue(Color.DarkBlue.ToVector3());

foreach (var mesh in Model.Meshes)
{
    Effect.Parameters["World"].SetValue(mesh.ParentBone.Transform * World);
    mesh.Draw();
}
````