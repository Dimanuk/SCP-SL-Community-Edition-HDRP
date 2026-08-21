using System.Collections.Generic;

namespace InventorySystem.Items.Firearms.Attachments.Formatters
{
    public static class AttachmentParameterFormatters
    {
        public static readonly Dictionary<AttachmentParam, IAttachmentsParameterFormatter> Formatters;

        static AttachmentParameterFormatters()
        {
            Formatters = new Dictionary<AttachmentParam, IAttachmentsParameterFormatter>
            {
                // ── Zoom (AdsZoom = 1) ──────────────────────────────────── //
                [(AttachmentParam)1] = new ZoomParameterFormatter(),

                // ── Standard multipliers, more is better ────────────────── //
                [(AttachmentParam)2] = new StandardParameterFormatter(moreIsBetter: true, isMultiplier: true, formatAsPercent: true),
                [(AttachmentParam)3] = new StandardParameterFormatter(moreIsBetter: true, isMultiplier: true, formatAsPercent: true),
                [(AttachmentParam)4] = new StandardParameterFormatter(moreIsBetter: true, isMultiplier: true, formatAsPercent: true),

                // ── Standard multipliers, less is better (weight, length) ── //
                [(AttachmentParam)5] = new StandardParameterFormatter(moreIsBetter: false, isMultiplier: true, formatAsPercent: true),
                [(AttachmentParam)6] = new StandardParameterFormatter(moreIsBetter: false, isMultiplier: true, formatAsPercent: true),

                // ── Inaccuracy (hip, ADS, moving) ────────────────────────── //
                [(AttachmentParam)7] = new InaccuracyParameterFormatter(),
                [(AttachmentParam)8] = new InaccuracyParameterFormatter(),
                [(AttachmentParam)9] = new InaccuracyParameterFormatter(),

                // ── Draw / holster time ──────────────────────────────────── //
                [(AttachmentParam)10] = new DrawParameterFormatter(),
                [(AttachmentParam)13] = new DrawParameterFormatter(),

                // ── Standard, less is better (e.g. recoil) ──────────────── //
                [(AttachmentParam)11] = new StandardParameterFormatter(moreIsBetter: false, isMultiplier: true, formatAsPercent: true),

                // ── Standard, additive, more is better (e.g. bullet count) ─ //
                [(AttachmentParam)12] = new StandardParameterFormatter(moreIsBetter: true, isMultiplier: false, formatAsPercent: false),

                [(AttachmentParam)14] = new StandardParameterFormatter(moreIsBetter: false, isMultiplier: false, formatAsPercent: false, suffix: "s"),

                // ── Standard multipliers, various sign conventions ─────────── //
                [(AttachmentParam)16] = new StandardParameterFormatter(moreIsBetter: true, isMultiplier: true, formatAsPercent: true),
                [(AttachmentParam)17] = new StandardParameterFormatter(moreIsBetter: false, isMultiplier: true, formatAsPercent: true),
                [(AttachmentParam)18] = new StandardParameterFormatter(moreIsBetter: true, isMultiplier: true, formatAsPercent: true),
                [(AttachmentParam)19] = new StandardParameterFormatter(moreIsBetter: true, isMultiplier: true, formatAsPercent: true),
                [(AttachmentParam)20] = new StandardParameterFormatter(moreIsBetter: true, isMultiplier: true, formatAsPercent: true),
            };
        }
    }
}
