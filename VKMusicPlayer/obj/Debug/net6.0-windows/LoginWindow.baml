   M S B A M L   `   `   `  ����H  DVKMusicPlayer, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null-clr-namespace:VKMusicPlayerVKMusicPlayer  5   	       VKMusicPlayer.LoginWindow   5   	   W SPresentationCore, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   -localclr-namespace:VKMusicPlayer     5   	     � Title$  LoginWindow��5   	    � SizeToContent$ WidthAndHeight=�6    � 
ResizeMode$ CanMinimize=�6<   .��5	      � $	��150��6   $	��400��6   .+�5
   
   �� $F��@Введите данные авторизации для vk.com��6   $
��Left=�6@   $��	75,10,0,0q�6[   $	��Top=�6n   �� 5   
   $��Логин:��6   $
��Left=�6!   $��	75,41,0,0q�6<   $	��Top=�6O   �� 5   
   $��Пароль:��6   $
��Left=�6"   $��	75,72,0,0q�6=   $	��Top=�6P   �� 5   
   -   5         ��Name$ 
VKLoginBox��5      $
��Left=�6&   $��
152,45,0,0q�6A    �� TextWrapping$
 Wrap=�6U   $	��Top=�6i   $	��159��6�   G� 5   
   -   5         G�Name$ 	VKPassBox��5      $
��Left=�6)   $��
152,76,0,0q�6D   $	��Top=�6X   $	��159��6p   �� 5   
   -   5         ��Name$ 
LoginEnter��5      $��
Войти��6%   $
��Left=�65   $��213,101,0,0q�6P   $	��Top=�6e    �� 	IsDefault$
 True��6}   6�   �� 5   
   -   5         ��Name$ SavePassCheckBox��5      $%��Запомнить пароль��6-   $
��Left=�6H   $��170,127,0,0q�6c   $	��Top=�6x   6�   5      5      