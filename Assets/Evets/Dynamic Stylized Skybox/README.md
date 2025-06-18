# Info
A simple, dynamic, and highly customizable skybox tool.

## Cubemap Texture Customizations
The skybox is designed around stylized textures. 
There are 4 elements that can be, **and is recommended to be**, customized:
1. **Clouds**: Stylizing clouds is a great way to make the skybox more visually appealing.
2. **High-altitude Clouds**: This is a secondary layer of clouds that can be used to add depth to the skybox.
3. **Moon**: The moon texture can be stylized too if you want it to be an emphasis in your game.
4. **Stars**: The stars texture can be customized to fit the theme of your game as well.

You can find sample textures for each elements under `Assets/Evets Vault/Stylized Dynamic Skybox/Textures`.

### Creating Cubemap Textures
You can refer to this helpful guide on how to create cubemap textures in Photoshop by Ciro Continisio:
https://www.youtube.com/watch?v=1b0g2j4k7aY

> **Please note that the video uses the 3D feature found only on older version of Photoshop (v22.0 and below).
You can also refer to other resources on how to create cubemap textures.**

Unity's documentation on cubemap textures can be found here:
https://docs.unity3d.com/6000.1/Documentation/Manual/class-Cubemap-create.html

### Included Textures
There are 4 included textures:
- 2048x4096 Painted Cloud Texture
- 2048x4096 Painted Cloud Back Texture
- 2048x4096 NASA Moon Surface Texture
- 4096x8192 NASA Starmap Texture
Refer to NASA visualization studio for reproduction guidelines: https://www.nasa.gov/nasa-brand-center/images-and-media/.

## Skybox Settings Scriptable Object
In the `Skybox Settings` scriptable object, you will find all the parameters to the skybox.
The script can be found at `Assets/Evets Vault/Stylized Dynamic Skybox/Scripts/SkyboxSettings.cs`.
You can further customize each variable by editing their set ranges found inside the script.

## Sky Prefab Script
The angle of the sun/moon is controlled by the `SkyboxController` script found in the `Sky Prefab` prefab. 
Script location is `Assets/Evets Vault/Stylized Dynamic Skybox/Scripts/SkyboxController.cs`.
If you want to dynamically update the angle of the celestial bodies, you can either write your own script to reference the `Sun` and `Moon` GameObjects or modify the `SkyboxController` script to suit your needs.