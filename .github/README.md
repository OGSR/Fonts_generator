# Fonts generator [![](https://img.shields.io/github/release/OGSR/Fonts_generator.svg?style=for-the-badge)](https://github.com/OGSR/Fonts_generator/releases/latest) #

**RU**: Утилита для создания шрифтов для **X-Ray Engine** ( `.dds` текстура и `.ini` конфиг ).
После создания шрифта его нужно добавить в `fonts.ltx`:
```ini
[new_font]
; Шейдер для ТЧ - формата текстуры шрифтов (рекомендуется выбирать в генераторе именно его, т.к. ЗП-формат текстур шрифтов менее качественный). В ЗП используется шейдер `hud\font`.
shader = font
texture = ui\new_font_texture
```

**EN**: Utility for creating fonts for **X-Ray Engine** ( `.dds` texture and `.ini` config).
After creating the font, you need to add it to `fonts.ltx`:
```ini
[new_font]
; Shader for SHOC - font texture format (it is recommended to select this one in the generator, since the CoP format of font textures is of lower quality). The CoP uses `hud\font` shader.
shader = font
texture = ui\new_font_texture
```