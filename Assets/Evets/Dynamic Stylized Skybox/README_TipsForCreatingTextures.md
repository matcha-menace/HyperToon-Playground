## Tips for creating cubemap textures:
Please see `/Textures/sample_clouds` for example.
There are a few requirements for the clouds texture:

- It must be in black and white! Think of it as mask, black part for the sky, and white parts for the clouds. It is okay to texture the clouds, but leaving it white makes it easier to colorize dynamically later.
- It must be a cubemap projection. See below for tips on how to paint your own cubemap projection.

To paint in 2D then have Photoshop project it easily for you, you need to use a version of Photoshop that's older than 2021.
Here's a helpful tutorial video: https://www.youtube.com/watch?v=XZmr-XYRw3w.

You can find Unity's documentation on cubemaps here:
https://docs.unity3d.com/6000.1/Documentation/Manual/class-Cubemap-create.html

### Included Textures
For all the included smaple textures, please refer to NASA visualization studio for reproduction guidelines: https://www.nasa.gov/nasa-brand-center/images-and-media/.

## Tips for modifying scripts
All of the skybox is controlled via one shader found in `/Scripts/EvetsSkybox.shader`, parameters of the shader is controlled via 2 scripts. **Feel free to freely modify any of the scripts to suit your needs.** They can be modified at runtime.
- SkyboxController.cs located in `/Scripts/SkyboxController.cs`
  - Included in the `Sky` prefab, simply passing in the transform data of Sun / Moons object rotations to the shader, and match Direction Light with the dominant celestial body.
- SkyboxSettings.cs located in `/Scripts/SkyboxSettings.cs`
  - A simple Scriptable Object which holds all the parameters for the skybox shader, such as the color of the sky, the color of the clouds, the speed of the clouds, etc.

_Please note that there is a custom Editor script found in `/Editor/SkyboxGUI.cs` that overrides the Inspector window of `SkyboxSettings`, feel free to disable that if you are making your own edits to `SkyboxSettings.cs`_

## Support

For support please contact me at: evets.dev@gmail.com

Email me some of your creations too if you want to! Would love to see them.

★★★★★
Please leave more reviews! Thank you for supporting <3

Unity Store Link: https://assetstore.unity.com/packages/slug/322177