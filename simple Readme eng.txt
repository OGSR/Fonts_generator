put text xml files(in utf-8) in xmlfiles folder, open config.ini do as instruction and use one of 3 bat file, a gamedata folder will show up when everything is done. Although fonts.ltx and localization.ltx may need some fix but font textures are ready.

it's better if you use ones with "noRequire_MSYH_ttf" because the old one(Generator.bat) verifies if there is MSYH font in windows path in the pre win7 way, which won't work in line win10, so I removed it in newer bat.

still you need MSYH font(one of the most famous chinese font) in your system to make it work.
of course you can edit bat to use your own font.