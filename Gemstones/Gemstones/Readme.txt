The algorithm I'm working on is currently having a bug: It will either remove gems having L shape OR 2 kinds of gems that are on the same line.
Cause: Line 64 & 117 in Gem.cs, at the second expression of the IF statement.

A new algorithm which I've just come up with is to sweep through all the board, if it has gems to be removed, add them to a list<gem>.
After the sweeping is completed, call a method that sweep all the list<gem>, and reduce their scale by 0.1f until they're all gone.

Log:
6/3/2013:
	- A gem containing nothing will always be swapped to the top of the board.
	- Có thể mô hình các cột bị sụp bằng 1 struct ColToBeMoved gồm 1 list các ngọc trong 1 cột sẽ bị sập và 1 biến integer lưu trữ số ô mà các viên ngọc trong
	list sẽ sập xuống.
	- Sau khi lấy được tất cả các list các cột như vậy, tiến hành do animation cho các cột đó bằng cách loop qua 1 ArrayList các ColToBeMoved, cuối
	mỗi loop cho thread nghỉ 60ms (framerate). Nếu integer trong ColToBeMoved đó bằng 0 thì continue (nghĩa là cột đó đã sập đến mức thấp nhất rồi,
	ko thể sập được nữa). Làm như vậy sẽ đảm bảo được framerate cho việc animation tất cả các viên ngọc cần sập.
	- Khi đã xử lý xong animation chạy đúng, ta tiến hành swap vị trí, thứ tự các phần tử (pointer) trong board. Có thể thực hiện việc này bằng cách
	viết 1 phương thức:
		Input: vị trí (Rectangle bounds, Point) các gem vừa di chuyển
		Output: vị trí index của chúng trong board, có được bằng cách chia input cho bề rộng của mỗi ô quân cờ (hằng số, trong game là 70px).
	Sau khi cập nhật xong các ngọc trong cột có giá trị, ta tiến hành cập nhật index cho gem.Nothing. Chỉ việc cập nhật các ô rỗng đó lên đầu các cột
	là xong.