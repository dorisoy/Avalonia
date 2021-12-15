using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Input.Raw;
using Avalonia.Layout;
using Avalonia.Platform;
using Avalonia.Rendering;
using Avalonia.UnitTests;
using Moq;
using Xunit;

namespace Avalonia.Input.UnitTests
{
    public class GesturesTests
    {
        private MouseTestHelper _mouse = new MouseTestHelper();

        [Fact]
        public void Tapped_Should_Follow_Pointer_Pressed_Released()
        {
            Border border = new Border();
            var decorator = new Decorator
            {
                Child = border
            };
            var result = new List<string>();

            AddHandlers(decorator, border, result, false);

            _mouse.Click(border);

            Assert.Equal(new[] { "bp", "dp", "br", "dr", "bt", "dt" }, result);
        }

        [Fact]
        public void Tapped_Should_Be_Raised_Even_When_Pressed_Released_Handled()
        {
            Border border = new Border();
            var decorator = new Decorator
            {
                Child = border
            };
            var result = new List<string>();

            AddHandlers(decorator, border, result, true);

            _mouse.Click(border);

            Assert.Equal(new[] { "bp", "dp", "br", "dr", "bt", "dt" }, result);
        }

        [Fact]
        public void Tapped_Should_Not_Be_Raised_For_Middle_Button()
        {
            Border border = new Border();
            var decorator = new Decorator
            {
                Child = border
            };
            var raised = false;

            decorator.AddHandler(Gestures.TappedEvent, (s, e) => raised = true);

            _mouse.Click(border, MouseButton.Middle);

            Assert.False(raised);
        }

        [Fact]
        public void Tapped_Should_Not_Be_Raised_For_Right_Button()
        {
            Border border = new Border();
            var decorator = new Decorator
            {
                Child = border
            };
            var raised = false;

            decorator.AddHandler(Gestures.TappedEvent, (s, e) => raised = true);

            _mouse.Click(border, MouseButton.Right);

            Assert.False(raised);
        }

        [Fact]
        public void RightTapped_Should_Be_Raised_For_Right_Button()
        {
            Border border = new Border();
            var decorator = new Decorator
            {
                Child = border
            };
            var raised = false;

            decorator.AddHandler(Gestures.RightTappedEvent, (s, e) => raised = true);

            _mouse.Click(border, MouseButton.Right);

            Assert.True(raised);
        }

        [Fact]
        public void DoubleTapped_Should_Follow_Pointer_Pressed_Released_Pressed()
        {
            Border border = new Border();
            var decorator = new Decorator
            {
                Child = border
            };
            var result = new List<string>();

            AddHandlers(decorator, border, result, false);

            _mouse.Click(border);
            _mouse.Down(border, clickCount: 2);

            Assert.Equal(new[] { "bp", "dp", "br", "dr", "bt", "dt", "bp", "dp", "bdt", "ddt" }, result);
        }

        [Fact]
        public void DoubleTapped_Should_Be_Raised_Even_When_Pressed_Released_Handled()
        {
            Border border = new Border();
            var decorator = new Decorator
            {
                Child = border
            };
            var result = new List<string>();

            AddHandlers(decorator, border, result, true);

            _mouse.Click(border);
            _mouse.Down(border, clickCount: 2);

            Assert.Equal(new[] { "bp", "dp", "br", "dr", "bt", "dt", "bp", "dp", "bdt", "ddt" }, result);
        }

        [Fact]
        public void DoubleTapped_Should_Not_Be_Raised_For_Middle_Button()
        {
            Border border = new Border();
            var decorator = new Decorator
            {
                Child = border
            };
            var raised = false;

            decorator.AddHandler(Gestures.DoubleTappedEvent, (s, e) => raised = true);

            _mouse.Click(border, MouseButton.Middle);
            _mouse.Down(border, MouseButton.Middle, clickCount: 2);

            Assert.False(raised);
        }

        [Fact]
        public void DoubleTapped_Should_Not_Be_Raised_For_Right_Button()
        {
            Border border = new Border();
            var decorator = new Decorator
            {
                Child = border
            };
            var raised = false;

            decorator.AddHandler(Gestures.DoubleTappedEvent, (s, e) => raised = true);

            _mouse.Click(border, MouseButton.Right);
            _mouse.Down(border, MouseButton.Right, clickCount: 2);

            Assert.False(raised);
        }
        private class TestTopLevel : Window
        {
            private readonly ILayoutManager _layoutManager;
            public bool IsClosed { get; private set; }

            public TestTopLevel(IWindowImpl impl, ILayoutManager layoutManager = null)
                : base(impl)
            {
                _layoutManager = layoutManager ?? new LayoutManager(this);
            }

            protected override ILayoutManager CreateLayoutManager() => _layoutManager;
        }
        [Fact]
        public void PointerPressedEventArgs_ClickCount_Should_Work_With_Triple_Clicks()
        {
            var inputManagerMock = new Mock<InputManager>();
            var services = TestServices.StyledWindow.With(inputManager: inputManagerMock.Object);

            using (UnitTestApplication.Start(services))
            {
              
                var impl = new Mock<IWindowImpl>(); 
                impl.SetupAllProperties();

                var target = new TestTopLevel(impl.Object);
                var tb= new TextBox() { Width = 50, Height = 50 };
                target.Content = tb;

                var input = new RawPointerEventArgs(
                    new Mock<MouseDevice>(null).Object,
                    0,
                    target,
                   RawPointerEventType.XButton2Down,
                   new Point(tb.Width / 2, tb.Height / 2), RawInputModifiers.None);
                impl.Object.Input(input);

            }
        }

        private void AddHandlers(
            Decorator decorator,
            Border border,
            IList<string> result,
            bool markHandled)
        {
            decorator.AddHandler(Border.PointerPressedEvent, (s, e) =>
            {
                result.Add("dp");

                if (markHandled)
                {
                    e.Handled = true;
                }
            });

            decorator.AddHandler(Border.PointerReleasedEvent, (s, e) =>
            {
                result.Add("dr");

                if (markHandled)
                {
                    e.Handled = true;
                }
            });

            border.AddHandler(Border.PointerPressedEvent, (s, e) => result.Add("bp"));
            border.AddHandler(Border.PointerReleasedEvent, (s, e) => result.Add("br"));

            decorator.AddHandler(Gestures.TappedEvent, (s, e) => result.Add("dt"));
            decorator.AddHandler(Gestures.DoubleTappedEvent, (s, e) => result.Add("ddt"));
            border.AddHandler(Gestures.TappedEvent, (s, e) => result.Add("bt"));
            border.AddHandler(Gestures.DoubleTappedEvent, (s, e) => result.Add("bdt"));
        }
    }
}
