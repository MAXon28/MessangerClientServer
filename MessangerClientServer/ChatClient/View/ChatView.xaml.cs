using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ChatClient.ViewModel;

namespace ChatClient.View
{
    public partial class ChatView : UserControl
    {
        private int _countZero;
        private ScrollViewer _scroll;

        public ChatView()
        {
            InitializeComponent();
        }

        public void ScrollStart()
        {
            _countZero = 1;
            MessageListView.SelectedIndex = MessageListView.Items.Count - 1;
            MessageListView.ScrollIntoView(MessageListView.SelectedItem);
        }

        public void Scroll()
        {
            _scroll.ScrollToEnd();
        }

        public void Scroll(int index)
        {
            MessageListView.SelectedIndex = index;
            MessageListView.ScrollIntoView(MessageListView.SelectedItem);
        }

        private void OnScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            var viewModel = (ChatViewModel)DataContext;
            double pixelInList = e.VerticalOffset;


            if (viewModel.IsVisibleButtonToBottom != pixelInList < e.ExtentHeight - 928)
            {
                viewModel.ShowButtonToBottom(pixelInList < e.ExtentHeight - 928);
            }


            if (_scroll == null)
            {
                _scroll = FindScrollViewer(MessageListView);
                _scroll?.ScrollToEnd();
            }

            _countZero += Convert.ToInt32(pixelInList) == 0 ? 1 : -_countZero;
            if (_countZero == 1)
            {
                viewModel.UpdateMessages();
            }
        }


        /// <summary>
        /// Поиск объекта типа ScrollViewer в ListView
        /// </summary>
        /// <param name="root"> В этом случае объект ListView</param>
        /// <returns> Возвращает ScrollViewer данного ListView </returns>
        private static ScrollViewer FindScrollViewer(DependencyObject root)
        {
            var queue = new Queue<DependencyObject>(new[] { root });

            do
            {
                var item = queue.Dequeue();

                if (item is ScrollViewer)
                {
                    return (ScrollViewer) item;
                }

                for (var i = 0; i < VisualTreeHelper.GetChildrenCount(item); i++)
                {
                    queue.Enqueue(VisualTreeHelper.GetChild(item, i));
                }
            } while (queue.Count > 0);

            return null;
        }
    }
}