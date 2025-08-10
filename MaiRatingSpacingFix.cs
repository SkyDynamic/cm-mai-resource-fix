using CardMaker.Common;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

namespace cm_mai_resource_fix
{
    public class MaiRatingSpacingFix
    {
        public static bool Enable = false;
        private static float originalSpacing = 0.0f;
        
        public static bool ShouldModifyCharSpacing(Sprite sprite)
        {
            if (sprite != null && sprite.name == "UI_NUM_26pt_Rating_00" && Enable)
            {
                return true;
            }
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(MU3UICounter), "OnPopulateMesh")]
        public static bool OnPopulateMesh_Prefix(MU3UICounter __instance, VertexHelper toFill)
        {
            if (!ShouldModifyCharSpacing(__instance.sprite))
            {
                return true;
            }
            toFill.Clear();
            if (null == __instance.sprite)
                return false;
            Rect rect = __instance.rectTransform.rect;
            Vector2 anchorPivot = MU3UICounterBase.getAnchorPivot(
                (MU3UICounterBase.Align)AccessTools.Field(typeof(MU3UICounterBase), "align_").GetValue(__instance));
            var calcTotalSizeInfo = AccessTools.Method(typeof(MU3UICounter), "calcTotalSize");
            Vector2 b = (Vector2) calcTotalSizeInfo.Invoke(__instance, null);
            Vector2 vector2_1 = Vector2.Scale(__instance.rectTransform.pivot, rect.size) - rect.size * 0.5f + Vector2.Scale(anchorPivot, b);
            float iw = 1f / __instance.sprite.texture.width;
            float ih = 1f / __instance.sprite.texture.height;
            Vector4 noPaddingOuterUv = MU3UICounterBase.GetNoPaddingOuterUV(__instance.sprite, iw, ih);
            float x1 = -vector2_1.x;

            var sizeInfo = AccessTools.Field(typeof(MU3UICounter), "size_");
            var signSizeInfo = AccessTools.Field(typeof(MU3UICounter), "signSize_");
            var size = (Vector2)sizeInfo.GetValue(__instance);
            var signSize = (Vector2)signSizeInfo.GetValue(__instance);
            float num1 = size.y - signSize.y;
            float num2;
            float num3;
            if (0.0 <= num1)
            {
                num2 = -vector2_1.y;
                num3 = (float) (-(double) vector2_1.y + 0.5 * num1);
            }
            else
            {
                num2 = (float) (-(double) vector2_1.y - 0.5 * num1);
                num3 = -vector2_1.y;
            }
            var charSpacingField = AccessTools.Field(typeof(MU3UICounter), "charSpacing_");
            originalSpacing = (float)charSpacingField.GetValue(__instance);
            
            Vector2 vector2_2 = size;
            float num4 = 0.0f;
            float num5 = originalSpacing;
            Color color = __instance.color;
            bool zeropadding = false;
            
            var numFiguresInfo = AccessTools.Field(typeof(MU3UICounter), "numFigures_");
            int numFigures = (int)numFiguresInfo.GetValue(__instance);
            var figuresInfo = AccessTools.Field(typeof(MU3UICounter), "figures_");
            var figures = (byte[])figuresInfo.GetValue(__instance);
            var ii = 0;
            for (int index = numFigures - 1; 0 <= index; --index)
            {
                float num6 = 0.0f;
                float x2;
                float y1;
                float y2;
                switch (figures[index])
                {
                    case 10:
                    case 12:
                        x2 = x1 + signSize.x;
                        y1 = signSize.y + num3;
                        y2 = num3;
                        break;
                    case 11:
                        var cammaSidePaddingInfo = AccessTools.Field(typeof(MU3UICounter), "cammaSidePadding_");
                        var cammaSidePadding = (float)cammaSidePaddingInfo.GetValue(__instance);
                        var cammaYOffsetInfo = AccessTools.Field(typeof(MU3UICounter), "cammaYOffset_");
                        var cammaYOffset = (float)cammaYOffsetInfo.GetValue(__instance);
                        x1 += cammaSidePadding;
                        num6 = cammaSidePadding;
                        x2 = x1 + signSize.x;
                        y1 = signSize.y + num3 + cammaYOffset;
                        y2 = num3 + cammaYOffset;
                        break;
                    case 13:
                        var decimalDotSidePaddingInfo = AccessTools.Field(typeof(MU3UICounter), "decimalDotSidePadding_");
                        var decimalDotSidePadding = (float)decimalDotSidePaddingInfo.GetValue(__instance);
                        var decimalDotYOffsetInfo = AccessTools.Field(typeof(MU3UICounter), "decimalDotYOffset_");
                        var decimalDotYOffset = (float)decimalDotYOffsetInfo.GetValue(__instance);
                        var decimalScaleInfo = AccessTools.Field(typeof(MU3UICounter), "decimalScale_");
                        var decimalScale = (Vector2)decimalScaleInfo.GetValue(__instance);
                        var decimalCharSpacingInfo = AccessTools.Field(typeof(MU3UICounter), "decimalCharSpacing_");
                        var decimalCharSpacing = (float)decimalCharSpacingInfo.GetValue(__instance);
                        
                        x1 += decimalDotSidePadding;
                        num6 = decimalDotSidePadding;
                        x2 = x1 + signSize.x;
                        y1 = signSize.y + num3;
                        y2 = num3 + decimalDotYOffset;
                        vector2_2 = Vector2.Scale(decimalScale, size);
                        num4 = decimalDotYOffset;
                        if (!__instance.DecimalUseIntegerSpacing)
                        {
                            num5 = decimalCharSpacing;
                        }
                        break;
                    case 14:
                        x1 = x1 + vector2_2.x + num5;
                        continue;
                    case 15:
                        zeropadding = __instance.ZeroPaddingOtherUV;
                        continue;
                    case 16:
                        zeropadding = false;
                        continue;
                    default:
                        x2 = x1 + vector2_2.x;
                        y1 = vector2_2.y + num2 + num4;
                        y2 = num2 + num4;
                        break;
                }
                Vector4 localUv = MU3UICounterBase.toLocalUV(noPaddingOuterUv, MU3UICounterBase.getNormalizedUV(figures[index], zeropadding), iw, ih);
                var tempVerticesInfo = AccessTools.Field(typeof(MU3UICounter), "tempVertices_");
                var tempVertices = (UIVertex[])tempVerticesInfo.GetValue(__instance);

                var padding = 0F;
                if (ii == 0) padding = 0.0F; 
                else if (ii == 1) padding = -1.6F;
                else if (ii == 2) padding = -3.0F;
                else if (ii == 3) padding = -4.6F;
                else if (ii == 4) padding = -5.4F;
                
                tempVertices[0].position = new Vector3(x1 + padding, y1);
                tempVertices[0].uv0 = new Vector2(localUv.x, localUv.w);
                tempVertices[0].color = color;
                tempVertices[1].position = new Vector3(x2 + padding, y1);
                tempVertices[1].uv0 = new Vector2(localUv.z, localUv.w);
                tempVertices[1].color = color;
                tempVertices[2].position = new Vector3(x2 + padding, y2);
                tempVertices[2].uv0 = new Vector2(localUv.z, localUv.y);
                tempVertices[2].color = color;
                tempVertices[3].position = new Vector3(x1 + padding, y2);
                tempVertices[3].uv0 = new Vector2(localUv.x, localUv.y);
                tempVertices[3].color = color;
                toFill.AddUIVertexQuad(tempVertices);
                x1 = x2 + num5 + num6;
                ii++;
            }

            return false;
        }
    }
}