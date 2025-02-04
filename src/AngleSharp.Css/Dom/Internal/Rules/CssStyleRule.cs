namespace AngleSharp.Css.Dom
{
    using AngleSharp.Css;
    using AngleSharp.Dom;
    using System;
    using System.Diagnostics;
    using System.IO;

    /// <summary>
    /// Represents a CSS style rule.
    /// </summary>
    [DebuggerDisplay(null, Name = "CssStyleRule ({SelectorText})")]
    sealed class CssStyleRule : CssRule, ICssStyleRule
    {
        #region Fields

        private readonly CssStyleDeclaration _style;
        private ISelector _selector;

        #endregion

        #region ctor

        internal CssStyleRule(ICssStyleSheet owner)
            : base(owner, CssRuleType.Style)
        {
            _style = new CssStyleDeclaration(this);
        }

        #endregion

        #region Properties

        public ISelector Selector => _selector;

        public String SelectorText
        {
            get => _selector?.Text;
            set => _selector = ParseSelector(value);
        }

        ICssStyleDeclaration ICssStyleRule.Style => _style;

        public CssStyleDeclaration Style => _style;

        #endregion

        #region Methods

        internal void SetInvalidSelector(String selectorText)
        {
            _selector = new InvalidSelector(selectorText);
        }

        protected override void ReplaceWith(ICssRule rule)
        {
            var newRule = (ICssStyleRule)rule;
            _selector = newRule.Selector;
            _style.SetDeclarations(newRule.Style);
        }

        public override void ToCss(TextWriter writer, IStyleFormatter formatter)
        {
            var block = _style.ToCssBlock(formatter);
            writer.Write(formatter.Rule(SelectorText, null, block));
        }

        #endregion

        #region Selector

        class InvalidSelector : ISelector
        {
            private readonly String _text;

            public InvalidSelector(String text)
            {
                _text = text;
            }

            public String Text => _text;

            public Priority Specificity => Priority.Zero;

            public void Accept(ISelectorVisitor visitor)
            {
            }

            public Boolean Match(IElement element, IElement scope) => false;
        }

        #endregion
    }
}
