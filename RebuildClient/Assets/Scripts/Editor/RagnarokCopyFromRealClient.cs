﻿using System;
using System.IO;
using Assets.Scripts;
using Assets.Scripts.Editor;
using Assets.Scripts.MapEditor.Editor;
using UnityEditor;
using UnityEngine;

namespace Assets.Editor
{
    public class RagnarokCopyFromRealClient : EditorWindow
    {
        public static void TestCopy()
        {
            var dataDir = RagnarokDirectory.GetRagnarokDataDirectorySafe;

            Func<string, string> updateHeadName = (str) => str.Replace("머리", "");
            Func<string, string> updateBodyName = (str) => str.Replace("몸", "");

            var headPath = Path.Combine(dataDir, "palette/머리");
            var bodyPath = Path.Combine(dataDir, "palette/몸");

            if (Directory.Exists(headPath))
            {
                CopyFolder(headPath, "Assets/Sprites/Characters/HeadFemale/Palette/", false, false, "*_여_*.pal", updateHeadName);
                CopyFolder(headPath, "Assets/Sprites/Characters/HeadMale/Palette/", false, false, "*_남_*.pal", updateHeadName);
            }

            headPath = Path.Combine(dataDir, "palette/머리/costume_1");
            bodyPath = Path.Combine(dataDir, "palette/몸/costume_1");

            if (Directory.Exists(bodyPath))
            {
                CopyFolder(bodyPath, "Assets/Sprites/Characters/BodyFemale/Palette/", false, false, "*_여_*.pal", updateHeadName);
                CopyFolder(bodyPath, "Assets/Sprites/Characters/BodyMale/Palette/", false, false, "*_남_*.pal");
            }
        }

        private static string UpdateSpriteName(string name)
        {
            name = name.Replace("성직자_", "Acolyte_");
            name = name.Replace("궁수_", "Archer_");
            name = name.Replace("마법사_", "Mage_");
            name = name.Replace("상인_", "Merchant_");
            name = name.Replace("초보자_", "Novice_");
            name = name.Replace("검사_", "Swordsman_");
            name = name.Replace("도둑_", "Thief_");
            name = name.Replace("여_", "F_");
            name = name.Replace("남_", "M_");

            return name;
        }


        [MenuItem("Ragnarok/TestCopy")]
        public static void TestCopy2()
        {
            var dataDir = RagnarokDirectory.GetRagnarokDataDirectorySafe;
            CopyFolder(Path.Combine(dataDir, "sprite/인간족/성직자"), "Assets/Sprites/Weapons/Acolyte/", false, true, "*", UpdateSpriteName);
            CopyFolder(Path.Combine(dataDir, "sprite/인간족/궁수"), "Assets/Sprites/Weapons/Archer/", false, true, "*", UpdateSpriteName);
            CopyFolder(Path.Combine(dataDir, "sprite/인간족/마법사"), "Assets/Sprites/Weapons/Mage/", false, true, "*", UpdateSpriteName);
            CopyFolder(Path.Combine(dataDir, "sprite/인간족/상인"), "Assets/Sprites/Weapons/Merchant/", false, true, "*", UpdateSpriteName);
            CopyFolder(Path.Combine(dataDir, "sprite/인간족/초보자"), "Assets/Sprites/Weapons/Novice/", false, true, "*", UpdateSpriteName);
            CopyFolder(Path.Combine(dataDir, "sprite/인간족/검사"), "Assets/Sprites/Weapons/Swordsman/", false, true, "*", UpdateSpriteName);
            CopyFolder(Path.Combine(dataDir, "sprite/인간족/도둑"), "Assets/Sprites/Weapons/Thief/", false, true, "*", UpdateSpriteName);
            CopyFolder(Path.Combine(dataDir, "sprite/인간족/슈퍼노비스"), "Assets/Sprites/Weapons/SuperNovice/", false, true, "*", UpdateSpriteName);


            CopyFolder(Path.Combine(dataDir, "sprite/방패/성직자"), "Assets/Sprites/Shields/Acolyte/", false, true, "*", UpdateSpriteName);
            CopyFolder(Path.Combine(dataDir, "sprite/방패/궁수"), "Assets/Sprites/Shields/Archer/", false, true, "*", UpdateSpriteName);
            CopyFolder(Path.Combine(dataDir, "sprite/방패/마법사"), "Assets/Sprites/Shields/Mage/", false, true, "*", UpdateSpriteName);
            CopyFolder(Path.Combine(dataDir, "sprite/방패/상인"), "Assets/Sprites/Shields/Merchant/", false, true, "*", UpdateSpriteName);
            CopyFolder(Path.Combine(dataDir, "sprite/방패/초보자"), "Assets/Sprites/Shields/Novice/", false, true, "*", UpdateSpriteName);
            CopyFolder(Path.Combine(dataDir, "sprite/방패/검사"), "Assets/Sprites/Shields/Swordsman/", false, true, "*", UpdateSpriteName);
            CopyFolder(Path.Combine(dataDir, "sprite/방패/도둑"), "Assets/Sprites/Shields/Thief/", false, true, "*", UpdateSpriteName);
            CopyFolder(Path.Combine(dataDir, "sprite/방패/슈퍼노비스"), "Assets/Sprites/Shields/SuperNovice/", false, true, "*", UpdateSpriteName);
        }

        [MenuItem("Ragnarok/Copy data from client data folder", priority = 1)]
        public static void CopyClientData()
        {
            var dataDir = RagnarokDirectory.GetRagnarokDataDirectorySafe;

            if (dataDir == null)
            {
                var prompt = @"Before you continue, you will need to specify a directory containing the contents of an extracted data.grf. "
                             + "For this import process to work correctly, the files will need to have been extracted with the right locale and working korean file names.";

                if (!EditorUtility.DisplayDialog("Copy from RO Client", prompt, "Continue", "Cancel"))
                    return;

                RagnarokDirectory.SetDataDirectory();

                dataDir = RagnarokDirectory.GetRagnarokDataDirectorySafe;
                if (dataDir == null)
                    return;
            }

            bool TestPath(string fileName)
            {
                if (!File.Exists(Path.Combine(dataDir, fileName)))
                {
                    Debug.LogError($"Could not verify client data directory \"{dataDir}\" is valid. File checked: {fileName} ");
                    return false;
                }

                return true;
            }

            if (!TestPath("prontera.gat") || !TestPath(@"texture\워터\water000.jpg"))
                return;

            var prompt2 = @"This import process will copy files from your data folder into this project. "
                          + "Because this includes converting all maps and objects, expect this process to take more than an hour."
                          + "\n\nWhen complete, the lighting window will load where you can bake the lighting for all the scenes (accessible via 'Ragnarok->Lighting Manager'). "
                          + "You will also need to manually copy over your BGM into the music folder if you want music."
                          + "\n\nLastly, before you run you will need to use 'Ragnarok->Update Addressables' to make sure everything can load.";

            if (!EditorUtility.DisplayDialog("Copy from RO Client", prompt2, "Continue", "Cancel"))
                return;

            CopyFolder(Path.Combine(dataDir, "wav/"), "Assets/Sounds/", true);
            CopyFolder(Path.Combine(dataDir, "sprite/몬스터"), "Assets/Sprites/Monsters/");
            CopyFolder(Path.Combine(dataDir, "sprite/악세사리/남"), "Assets/Sprites/Headgear/Male/");
            CopyFolder(Path.Combine(dataDir, "sprite/악세사리/여"), "Assets/Sprites/Headgear/Female/");
            CopyFolder(Path.Combine(dataDir, "sprite/npc"), "Assets/Sprites/Npcs/");
            CopyFolder(Path.Combine(dataDir, "sprite/이팩트"), "Assets/Sprites/Effects/");
            CopyFolder(Path.Combine(dataDir, "sprite/인간족/머리통/남"), "Assets/Sprites/Characters/HeadMale/");
            CopyFolder(Path.Combine(dataDir, "sprite/인간족/머리통/여"), "Assets/Sprites/Characters/HeadFemale/");
            CopyFolder(Path.Combine(dataDir, "sprite/인간족/몸통/남"), "Assets/Sprites/Characters/BodyMale/");
            CopyFolder(Path.Combine(dataDir, "sprite/인간족/몸통/여"), "Assets/Sprites/Characters/BodyFemale/");
            CopyFolder(Path.Combine(dataDir, "palette/몸"), "Assets/Sprites/Characters/HeadFemale/", false, false, "*_여_*.pal");
            CopyFolder(Path.Combine(dataDir, "palette/몸"), "Assets/Sprites/Characters/HeadMale/", false, false, "*_남_*.pal");

            CopyFolder(Path.Combine(dataDir, "sprite/인간족/몸통/남"), "Assets/Sprites/Characters/BodyMale/");
            CopyFolder(Path.Combine(dataDir, "sprite/인간족/몸통/여"), "Assets/Sprites/Characters/BodyFemale/");
            CopyFolder(Path.Combine(dataDir, "texture/유저인터페이스/illust"), "Assets/Sprites/Cutins/");

            CopyFolder(Path.Combine(dataDir, "sprite/인간족/성직자"), "Assets/Sprites/Weapons/Acolyte/", false, true, "*", UpdateSpriteName);
            CopyFolder(Path.Combine(dataDir, "sprite/인간족/궁수"), "Assets/Sprites/Weapons/Archer/", false, true, "*", UpdateSpriteName);
            CopyFolder(Path.Combine(dataDir, "sprite/인간족/마법사"), "Assets/Sprites/Weapons/Mage/", false, true, "*", UpdateSpriteName);
            CopyFolder(Path.Combine(dataDir, "sprite/인간족/상인"), "Assets/Sprites/Weapons/Merchant/", false, true, "*", UpdateSpriteName);
            CopyFolder(Path.Combine(dataDir, "sprite/인간족/초보자"), "Assets/Sprites/Weapons/Novice/", false, true, "*", UpdateSpriteName);
            CopyFolder(Path.Combine(dataDir, "sprite/인간족/검사"), "Assets/Sprites/Weapons/Swordsman/", false, true, "*", UpdateSpriteName);
            CopyFolder(Path.Combine(dataDir, "sprite/인간족/도둑"), "Assets/Sprites/Weapons/Thief/", false, true, "*", UpdateSpriteName);
            CopyFolder(Path.Combine(dataDir, "sprite/인간족/슈퍼노비스"), "Assets/Sprites/Weapons/SuperNovice/", false, true, "*", UpdateSpriteName);
            CopyFolder(Path.Combine(dataDir, "sprite/인간족/기사"), "Assets/Sprites/Weapons/Knight/", false, true, "*", UpdateSpriteName);
            CopyFolder(Path.Combine(dataDir, "sprite/인간족/위저드"), "Assets/Sprites/Weapons/Wizard/", false, true, "*", UpdateSpriteName);
            CopyFolder(Path.Combine(dataDir, "sprite/인간족/프리스트"), "Assets/Sprites/Weapons/Priest/", false, true, "*", UpdateSpriteName);
            CopyFolder(Path.Combine(dataDir, "sprite/인간족/헌터"), "Assets/Sprites/Weapons/Hunter/", false, true, "*", UpdateSpriteName);
            CopyFolder(Path.Combine(dataDir, "sprite/인간족/어세신"), "Assets/Sprites/Weapons/Assassin/", false, true, "*", UpdateSpriteName);
            CopyFolder(Path.Combine(dataDir, "sprite/인간족/제철공"), "Assets/Sprites/Weapons/Blacksmith/", false, true, "*", UpdateSpriteName);
            CopyFolder(Path.Combine(dataDir, "sprite/인간족/크루세이더"), "Assets/Sprites/Weapons/Crusader/", false, true, "*", UpdateSpriteName);
            CopyFolder(Path.Combine(dataDir, "sprite/인간족/세이지"), "Assets/Sprites/Weapons/Sage/", false, true, "*", UpdateSpriteName);
            CopyFolder(Path.Combine(dataDir, "sprite/인간족/바드"), "Assets/Sprites/Weapons/Bard/", false, true, "*", UpdateSpriteName);
            CopyFolder(Path.Combine(dataDir, "sprite/인간족/무희바지"), "Assets/Sprites/Weapons/Dancer/", false, true, "*", UpdateSpriteName);
            CopyFolder(Path.Combine(dataDir, "sprite/인간족/몽크"), "Assets/Sprites/Weapons/Monk/", false, true, "*", UpdateSpriteName);
            CopyFolder(Path.Combine(dataDir, "sprite/인간족/로그"), "Assets/Sprites/Weapons/Rogue/", false, true, "*", UpdateSpriteName);
            CopyFolder(Path.Combine(dataDir, "sprite/인간족/연금술사"), "Assets/Sprites/Weapons/Alchemist/", false, true, "*", UpdateSpriteName);
            CopyFolder(Path.Combine(dataDir, "sprite/인간족/운영자"), "Assets/Sprites/Weapons/GameMaster/", false, true, "*", UpdateSpriteName);
            CopyFolder(Path.Combine(dataDir, "sprite/인간족/신페코크루세이더"), "Assets/Sprites/Weapons/PecoCrusader/", false, true, "*", UpdateSpriteName);
            CopyFolder(Path.Combine(dataDir, "sprite/인간족/페코페코_기사_남"), "Assets/Sprites/Weapons/PecoKnight/", false, true, "*", UpdateSpriteName);
            
            CopyFolder(Path.Combine(dataDir, "sprite/방패/성직자"), "Assets/Sprites/Shields/Acolyte/", false, true, "*", UpdateSpriteName);
            CopyFolder(Path.Combine(dataDir, "sprite/방패/궁수"), "Assets/Sprites/Shields/Archer/", false, true, "*", UpdateSpriteName);
            CopyFolder(Path.Combine(dataDir, "sprite/방패/마법사"), "Assets/Sprites/Shields/Mage/", false, true, "*", UpdateSpriteName);
            CopyFolder(Path.Combine(dataDir, "sprite/방패/상인"), "Assets/Sprites/Shields/Merchant/", false, true, "*", UpdateSpriteName);
            CopyFolder(Path.Combine(dataDir, "sprite/방패/초보자"), "Assets/Sprites/Shields/Novice/", false, true, "*", UpdateSpriteName);
            CopyFolder(Path.Combine(dataDir, "sprite/방패/검사"), "Assets/Sprites/Shields/Swordsman/", false, true, "*", UpdateSpriteName);
            CopyFolder(Path.Combine(dataDir, "sprite/방패/도둑"), "Assets/Sprites/Shields/Thief/", false, true, "*", UpdateSpriteName);
            CopyFolder(Path.Combine(dataDir, "sprite/방패/슈퍼노비스"), "Assets/Sprites/Shields/SuperNovice/", false, true, "*", UpdateSpriteName);
            CopyFolder(Path.Combine(dataDir, "sprite/방패/기사"), "Assets/Sprites/Shields/Knight/", false, true, "*", UpdateSpriteName);
            CopyFolder(Path.Combine(dataDir, "sprite/방패/위저드"), "Assets/Sprites/Shields/Wizard/", false, true, "*", UpdateSpriteName);
            CopyFolder(Path.Combine(dataDir, "sprite/방패/프리스트"), "Assets/Sprites/Shields/Priest/", false, true, "*", UpdateSpriteName);
            CopyFolder(Path.Combine(dataDir, "sprite/방패/헌터"), "Assets/Sprites/Shields/Hunter/", false, true, "*", UpdateSpriteName);
            CopyFolder(Path.Combine(dataDir, "sprite/방패/어세신"), "Assets/Sprites/Shields/Assassin/", false, true, "*", UpdateSpriteName);
            CopyFolder(Path.Combine(dataDir, "sprite/방패/제철공"), "Assets/Sprites/Shields/Blacksmith/", false, true, "*", UpdateSpriteName);
            CopyFolder(Path.Combine(dataDir, "sprite/방패/크루세이더"), "Assets/Sprites/Shields/Crusader/", false, true, "*", UpdateSpriteName);
            CopyFolder(Path.Combine(dataDir, "sprite/방패/세이지"), "Assets/Sprites/Shields/Sage/", false, true, "*", UpdateSpriteName);
            CopyFolder(Path.Combine(dataDir, "sprite/방패/바드"), "Assets/Sprites/Shields/Bard/", false, true, "*", UpdateSpriteName);
            CopyFolder(Path.Combine(dataDir, "sprite/방패/무희바지"), "Assets/Sprites/Shields/Dancer/", false, true, "*", UpdateSpriteName);
            CopyFolder(Path.Combine(dataDir, "sprite/방패/몽크"), "Assets/Sprites/Shields/Monk/", false, true, "*", UpdateSpriteName);
            CopyFolder(Path.Combine(dataDir, "sprite/방패/로그"), "Assets/Sprites/Shields/Rogue/", false, true, "*", UpdateSpriteName);
            CopyFolder(Path.Combine(dataDir, "sprite/방패/연금술사"), "Assets/Sprites/Shields/Alchemist/", false, true, "*", UpdateSpriteName);
            CopyFolder(Path.Combine(dataDir, "sprite/방패/운영자"), "Assets/Sprites/Shields/GameMaster/", false, true, "*", UpdateSpriteName);
            CopyFolder(Path.Combine(dataDir, "sprite/방패/신페코크루세이더"), "Assets/Sprites/Shields/PecoCrusader/", false, true, "*", UpdateSpriteName);
            CopyFolder(Path.Combine(dataDir, "sprite/방패/페코페코_기사_남"), "Assets/Sprites/Shields/PecoKnight/", false, true, "*", UpdateSpriteName);

            CopySingleFile(Path.Combine(dataDir, "sprite/cursors.act"), "Assets/Sprites/Misc/");
            CopySingleFile(Path.Combine(dataDir, "sprite/cursors.spr"), "Assets/Sprites/Misc/");
            CopySingleFile(Path.Combine(dataDir, "sprite/이팩트/emotion.act"), "Assets/Sprites/Misc/");
            CopySingleFile(Path.Combine(dataDir, "sprite/이팩트/emotion.spr"), "Assets/Sprites/Misc/");
            CopySingleFile(Path.Combine(dataDir, "sprite/이팩트/숫자.act"), "Assets/Sprites/Misc/damagenumbers.act");
            CopySingleFile(Path.Combine(dataDir, "sprite/이팩트/숫자.spr"), "Assets/Sprites/Misc/damagenumbers.spr");

            //the project has custom monsters, but for copyright reasons the sprites aren't part of the repo
            //to make things still run without the custom sprites we substitute a similar sprite if necessary
            CreateTemporarySpriteIfRequired("andre", "andre_larva");
            CreateTemporarySpriteIfRequired("deniro", "deniro_larva");
            CreateTemporarySpriteIfRequired("piere", "piere_larva");
            CreateTemporarySpriteIfRequired("andre", "soldier_andre");
            CreateTemporarySpriteIfRequired("deniro", "soldier_deniro");
            CreateTemporarySpriteIfRequired("piere", "soldier_piere");
            CreateTemporarySpriteIfRequired("vagabond_wolf", "were_wolf");
            CreateTemporarySpriteIfRequired("frilldora", "raptice");
            CreateTemporarySpriteIfRequired("poison_spore", "deathspore");

            AssetDatabase.Refresh();

            EffectStrImporter.Import(); //effects
            EffectStrImporter.ImportEffectTextures();
            RagnarokMapImporterWindow.ImportWater();
            RagnarokMapImporterWindow.ImportAllFiles();
            ItemIconImporter.ImportItems();

            RoLightingManagerWindow.CreateOrOpen();
        }

        private static void CreateTemporarySpriteIfRequired(string dummySpriteName, string spriteName)
        {
            var destSprPath = $"Assets/Sprites/Monsters/{spriteName}.spr";
            var destActPath = $"Assets/Sprites/Monsters/{spriteName}.act";
            if (File.Exists(destSprPath) || File.Exists(destActPath))
                return;
            var dataDir = RagnarokDirectory.GetRagnarokDataDirectorySafe;
            CopySingleFile(Path.Combine(dataDir, $"sprite/몬스터/{dummySpriteName}.spr"), destSprPath);
            CopySingleFile(Path.Combine(dataDir, $"sprite/몬스터/{dummySpriteName}.act"), destActPath);
        }

        private static void CopySingleFile(string src, string dest)
        {
            var fName = Path.GetFileName(dest);
            if (string.IsNullOrWhiteSpace(fName))
                dest = Path.Combine(dest, Path.GetFileName(src));

            var dir = Path.GetDirectoryName(dest);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            // Debug.Log($"${src}  :::  {dest}");

            if (!File.Exists(dest))
                File.Copy(src, dest, false);
        }

        private static bool CopyFolder(string src, string dest, bool recursive = false, bool maleFemaleSplit = false, string filter = "*",
            Func<string, string> updateFileName = null)
        {
            var opt = recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;

            var hasFiles = false;

            if (!Directory.Exists(dest))
                Directory.CreateDirectory(dest);

            foreach (var path in Directory.GetFiles(src, filter, opt))
            {
                var rel = Path.GetRelativePath(src, path);
                var destPath = Path.Combine(dest, rel);

                hasFiles = true;

                if (maleFemaleSplit)
                {
                    if (rel.Contains("_남_"))
                        destPath = Path.Combine(dest, "Male/", rel);
                    if (rel.Contains("_여_"))
                        destPath = Path.Combine(dest, "Female/", rel);
                }

                if (File.Exists(destPath.Replace(".bmp", ".png")))
                    continue;

                if (updateFileName != null)
                    destPath = updateFileName(destPath);

                var outDir = Path.GetDirectoryName(destPath);
                if (!Directory.Exists(outDir))
                    Directory.CreateDirectory(outDir);

                var ext = Path.GetExtension(path);
                var fName = Path.GetFileName(path);

                if (ext == ".bmp")
                {
                    var tex = TextureImportHelper.LoadTexture(path);
                    TextureImportHelper.SaveAndUpdateTexture(tex, destPath.Replace(".bmp", ".png"), ti =>
                    {
                        ti.textureType = TextureImporterType.Sprite;
                        ti.spriteImportMode = SpriteImportMode.Single;
                        ti.crunchedCompression = false;
                        ti.textureCompression = TextureImporterCompression.CompressedHQ;
                    });
                    //TextureImportHelper.GetOrImportTextureToProject(fName, path, destPath);
                }
                else
                {
                    if (!File.Exists(destPath))
                        File.Copy(path, destPath, false);
                }
            }

            AssetDatabase.Refresh();

            return hasFiles;
        }
    }
}