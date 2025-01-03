using Microsoft.Xna.Framework;

namespace MonoGameUI.Test;

public class MockGame : Game
{
    GraphicsDeviceManager graphics;

    public MockGame()
    {
        graphics = new(this)
        {
            PreferredBackBufferWidth = 800,
            PreferredBackBufferHeight = 480
        };
    }

    protected override void Update(GameTime gameTime)
    {
        // Exit the game after the first frame update
        Exit();

        base.Update(gameTime);
    }

}
