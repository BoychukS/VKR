   M S B A M L   `   `   `  ����  VkrProgramm)    #VkrProgramm.EditAttendanceAndGrades   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	     � Title$.  (Оценки и посещаемость��5   	   $	��550��6'   $	��750��64    � WindowStartupLocation$ CenterScreen=�6@   .��5      � $��10q�6    � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5	      $��* �6   �� 5
      $
��Auto �6   .+�5   
   ��  �� Orientation$ 
Horizontal=�6   $��0,0,0,10q�6.   .+�5      �� $��Группа:��6   $��Center=�6'   $��16(�6B   $��0,0,10,0q�6P   �� 5      -   5         ��Name$ cmbGroup��5      $	��200��6)   $��25��65   $��14(�6A    �� DisplayMemberPath$ 	GroupName��6O   6m   5      �� 5   
   $ 
Horizontal=�6   $��0,0,0,10q�6.   $��1��6@   .+�5      �� $��Занятие:��6   $��Center=�6(   $��16(�6C   $��0,0,10,0q�6Q   �� 5      -   5        $ cmbSchedule��5      $	��400��6,   $��25��68   $��14(�6D   $ DisplayName��6R   6r   5      &   System.Windows.Controls.DataGrid  5   
   -   5          Name$ dataGridStudents��5      $��2��6-      AutoGenerateColumns .     CanUserAddRows .  $��0,0,0,10q�6m   	   Columns	 5      0  *System.Windows.Controls.DataGridTextColumn  5      ,  &System.Windows.Controls.DataGridColumn
   Header$
 Студент��6%      
IsReadOnly .    Width5  /System.Windows.Controls.DataGridLengthConverter$ * 6h   1  +System.Windows.Controls.DataGridBoundColumn   Binding 66   �� *StudentName+6h     5      $
 Оценка��6%   $	 100 6t    65   �� *Grade+ �� UpdateSourceTrigger$ PropertyChanged=�6t   4  .System.Windows.Controls.DataGridCheckBoxColumn  5      $
 Посещение��6)   $	 100 6    6<   �� *	IsPresent+$ PropertyChanged=�6   5      �� 5   
   $��3��6   $ 
Horizontal=�6"   $��Right=�6;   $��0,0,0,0q�6W   .+�5       �� -   5        $��Сохранить��5       $	��120��6)   $��35��65   $��0,0,10,0q�6A   6S   �� 5!      -   5        $��Вернуться��5!      $	��150��6)   $��35��65   6A   5"      5#      5$      