# Hướng Dẫn Toàn Tập — App Quản Trị Giasu123 (Tiếng Việt)

> Tài liệu này viết cho **người mới hoàn toàn**: bạn là dev web, chưa biết
> WinForms, chưa biết SQL. Mình sẽ giải thích **từ số 0**, so sánh với thế giới
> web mà bạn đã quen, để bạn hiểu mọi thứ. Đọc một lượt từ trên xuống là nắm được.

---

## Mục lục
1. [WinForms là gì? Vì sao app trông giống "bảng dữ liệu"?](#1-winforms-là-gì)
2. [So sánh Web ↔ WinForms (phần quan trọng nhất cho bạn)](#2-so-sánh-web--winforms)
3. [SQL căn bản — học từ số 0](#3-sql-căn-bản--học-từ-số-0)
4. [Cơ sở dữ liệu của app (6 bảng)](#4-cơ-sở-dữ-liệu-của-app)
5. [Cấu trúc code — từng file làm gì](#5-cấu-trúc-code)
6. [CRUD — ý tưởng cốt lõi của cả app](#6-crud--ý-tưởng-cốt-lõi)
7. [Đọc hiểu file TutorsPage.cs từng bước](#7-đọc-hiểu-tutorspagecs)
8. [App chạy như thế nào (luồng từ lúc mở app)](#8-luồng-chạy-của-app)
9. [Code có phức tạp không?](#9-code-có-phức-tạp-không)
10. [Cách tự chỉnh sửa thường gặp](#10-cách-tự-chỉnh-sửa)
11. [Bảng tra thuật ngữ](#11-bảng-tra-thuật-ngữ)

---

## 1. WinForms là gì?

**WinForms** (Windows Forms) là công nghệ của Microsoft để làm **app chạy trực
tiếp trên máy tính Windows** (file `.exe`), không cần trình duyệt.

Bạn là dev web, nên hãy hình dung thế này:
- Web: bạn viết HTML/CSS/JS → trình duyệt hiển thị → chạy trên mọi thiết bị.
- WinForms: bạn viết C# → Windows hiển thị bằng các "nút, ô, bảng" có sẵn của hệ
  điều hành → chạy như một phần mềm desktop (giống Zalo PC, Word...).

### Vì sao app trông giống "database dashboard"?
Vì **đúng bản chất nó là vậy** 🙂. Đây là một app quản trị (admin). App quản trị
ở bất kỳ nền tảng nào (web hay desktop) đều có hình dạng giống nhau:

```
[ Menu ] [ Bảng danh sách dữ liệu ] [ Ô nhập để sửa ]
```

Giống như trang admin của WordPress, hay bảng điều khiển của một web app — chỉ
khác là ở đây nó là **ứng dụng desktop native** nên trông "vuông vức, kiểu
Windows" hơn. Đó là **bình thường và đúng chuẩn**. Giao diện WinForms mặc định
trông giống Windows vì nó dùng chính các thành phần của Windows.

> Tóm lại: App trông giống "database dashboard" **không phải do code dở**, mà vì
> đó chính là đúng loại app bạn đang làm (công cụ quản lý dữ liệu).

---

## 2. So sánh Web ↔ WinForms

Đây là bảng quan trọng nhất cho bạn. Mọi khái niệm web đều có khái niệm tương
đương trong WinForms:

| Thế giới Web (bạn đã quen) | WinForms (cái mới) | Trong project này |
|---|---|---|
| Thẻ HTML: `<input>`, `<button>`, `<table>` | **Control**: `TextBox`, `Button`, `DataGridView` | dùng khắp các file |
| CSS (màu, font, khoảng cách) | Thuộc tính của control + 1 file màu chung | `Theme.cs` |
| JavaScript xử lý sự kiện: `onclick` | C#: `button.Click += ...` | ví dụ trong `CrudPage.cs` |
| `fetch()` / `axios` gọi API | Gọi hàm trong `Database.cs` | `Database.Query(...)` |
| Backend + REST API | Không cần! App nối **thẳng** vào database | `Database.cs` |
| 1 trang HTML (`page.html`) | 1 "màn hình" = 1 class kế thừa `UserControl` | các file trong `Pages/` |
| Router (chuyển trang) | Sidebar đổi màn hình | `MainForm.cs` |
| Chạy trong trình duyệt | Chạy thành file `.exe` | `bin/.../Giasu123.exe` |

**Khác biệt lớn nhất với web:** app desktop này **nối thẳng vào PostgreSQL**,
không có "backend API" ở giữa. Trong web bạn hay có: Trình duyệt → API → Database.
Ở đây chỉ có: App → Database. Đơn giản hơn, nhưng vì vậy app chạy trong mạng nội
bộ/máy admin chứ không public ra Internet.

---

## 3. SQL căn bản — học từ số 0

**SQL** là "ngôn ngữ để nói chuyện với cơ sở dữ liệu". Database lưu dữ liệu trong
các **bảng** (table) — giống hệt một bảng Excel: có **cột** (column) và **dòng**
(row).

Ví dụ bảng `tutors` (gia sư):

| tutor_id | name      | subject | hourly_rate | level     |
|----------|-----------|---------|-------------|-----------|
| 1        | Nguyễn A  | Toán    | 200000      | Đại học   |
| 2        | Trần B    | Anh     | 150000      | Cao đẳng  |

Chỉ có **4 câu lệnh SQL** bạn cần biết (gọi là CRUD):

### 3.1. SELECT — Lấy / đọc dữ liệu (Read)
```sql
SELECT name, subject FROM tutors;
```
Dịch ra tiếng Việt: "**Lấy** cột `name`, `subject` **từ bảng** `tutors`".

Thêm điều kiện bằng `WHERE`:
```sql
SELECT * FROM tutors WHERE subject = 'Toán';
```
`*` nghĩa là "tất cả các cột". Câu này: "Lấy tất cả gia sư dạy Toán".

### 3.2. INSERT — Thêm dòng mới (Create)
```sql
INSERT INTO tutors (name, subject, hourly_rate, level)
VALUES ('Lê C', 'Lý', 180000, 'Đại học');
```
Dịch: "**Thêm vào** bảng `tutors` một dòng mới với các giá trị này".

### 3.3. UPDATE — Sửa dòng đã có (Update)
```sql
UPDATE tutors SET hourly_rate = 250000 WHERE tutor_id = 1;
```
Dịch: "**Cập nhật** gia sư có `tutor_id = 1`, đặt `hourly_rate` thành 250000".

> ⚠️ Luôn có `WHERE` khi UPDATE/DELETE! Nếu quên, nó sẽ sửa/xoá **toàn bộ** bảng.

### 3.4. DELETE — Xoá dòng (Delete)
```sql
DELETE FROM tutors WHERE tutor_id = 1;
```
Dịch: "**Xoá** gia sư có `tutor_id = 1`".

### 3.5. Vài từ khoá hay gặp trong code của app
| Từ khoá | Nghĩa |
|---------|-------|
| `ORDER BY tutor_id` | Sắp xếp kết quả theo cột `tutor_id` |
| `WHERE name ILIKE '%an%'` | Lọc tên có chứa chữ "an" (ILIKE = không phân biệt hoa/thường) |
| `JOIN` | Nối 2 bảng lại với nhau (xem phần khóa ngoại bên dưới) |
| `COUNT(*)` | Đếm số dòng |
| `SUM(amount)` | Tính tổng cột `amount` |
| `@id`, `@name` | "Tham số" — chỗ trống để điền giá trị an toàn (xem mục 7) |

Chỉ cần hiểu 4 câu CRUD ở trên là bạn đọc được 90% SQL trong app.

---

## 4. Cơ sở dữ liệu của app

Database tên là **`legal_text2sql`**, có **6 bảng**:

| Bảng | Ý nghĩa | Các cột |
|------|---------|---------|
| `tutors` | Gia sư | tutor_id, name, subject, hourly_rate, level |
| `students` | Học viên | student_id, name, grade, city |
| `classes` | Lớp học | class_id, subject, grade, fee, **tutor_id** |
| `enrollments` | Ghi danh (học viên vào lớp) | enrollment_id, **student_id**, **class_id**, enroll_date, status |
| `payments` | Thanh toán | payment_id, **enrollment_id**, amount, payment_date, method, status |
| `reviews` | Đánh giá gia sư | review_id, **tutor_id**, **student_id**, rating, comment, review_date |

### Khóa ngoại (Foreign Key) là gì?
Những cột **in đậm** ở trên là **khóa ngoại** — tức là cột "trỏ sang" một bảng
khác. Ví dụ: bảng `classes` có cột `tutor_id`, nghĩa là "lớp này do gia sư nào
dạy". Nó lưu số `tutor_id` của gia sư đó trong bảng `tutors`.

Giống như trong web khi bạn có `userId` để biết bài viết thuộc về user nào vậy.

Trong app, mỗi khóa ngoại được hiển thị thành một **ô chọn (dropdown)** để bạn
chọn **tên** thay vì gõ số id. Ví dụ ở màn hình Classes, ô "Tutor" là một dropdown
liệt kê tên các gia sư.

Sơ đồ liên kết:
```
tutors ──< classes ──< enrollments ──< payments
students ──────────────┘
students, tutors ──< reviews
```
(`──<` đọc là "một ... có nhiều ...": một gia sư có nhiều lớp, một lớp có nhiều
lượt ghi danh, v.v.)

---

## 5. Cấu trúc code

Mỗi file làm **đúng một việc**. Đây là bản đồ:

```
Giasu123/
├─ Program.cs       → điểm khởi động; kiểm tra kết nối DB trước khi mở cửa sổ
├─ Database.cs      → file DUY NHẤT nói chuyện với PostgreSQL
├─ Theme.cs         → toàn bộ màu sắc, font chữ (giống file CSS)
├─ MainForm.cs      → cửa sổ chính: thanh menu bên trái + vùng nội dung
└─ Pages/
   ├─ CrudPage.cs        → "khuôn mẫu" dùng chung cho mọi màn hình bảng
   ├─ DashboardPage.cs   → màn hình tổng quan (thẻ số liệu + thanh toán gần đây)
   ├─ TutorsPage.cs      → quản lý gia sư   ← FILE VÍ DỤ có chú thích đầy đủ
   ├─ StudentsPage.cs    → quản lý học viên
   ├─ ClassesPage.cs     → quản lý lớp (có dropdown Gia sư)
   ├─ EnrollmentsPage.cs → quản lý ghi danh (dropdown Học viên + Lớp)
   ├─ PaymentsPage.cs    → quản lý thanh toán (dropdown Ghi danh)
   └─ ReviewsPage.cs     → quản lý đánh giá (dropdown Gia sư + Học viên)
```

### Giải thích vai trò từng file

**`Database.cs` — "Cầu nối với database"**
Đây là nơi duy nhất chứa mật khẩu và địa chỉ database. Mọi màn hình khác muốn
lấy/ghi dữ liệu đều gọi qua đây. Nó có mấy hàm:
- `Query(sql)` → chạy SELECT, trả về bảng dữ liệu để hiển thị.
- `Execute(sql)` → chạy INSERT / UPDATE / DELETE.
- `Scalar(sql)` → lấy một con số duy nhất (ví dụ đếm số gia sư).
- `P("ten", giatri)` → tạo một "tham số" an toàn (xem mục 7).

Giống như file `api.js` trong web của bạn — tập trung mọi lời gọi dữ liệu một chỗ.

**`Theme.cs` — "File CSS của app"**
Chứa toàn bộ màu (xanh, trắng, xám...) và font. Muốn đổi màu cả app → sửa ở đây.

**`CrudPage.cs` — "Khuôn mẫu chung"**
Đây là phần "thông minh" nhất, nhưng bạn **không cần sửa** nó. Nó dựng sẵn bố cục
chung (tiêu đề, ô tìm kiếm, bảng, panel sửa, nút Save/Delete) để 6 màn hình kia
không phải viết lại. Xem mục 6.

**Các file trong `Pages/` — "Từng màn hình"**
Mỗi file quản lý 1 bảng. Chúng rất giống nhau, chỉ khác tên cột. Giống như trong
web bạn có nhiều trang CRUD na ná nhau vậy.

**`MainForm.cs` — "Khung + Router"**
Vẽ thanh menu bên trái. Bấm nút nào thì hiện màn hình tương ứng (giống chuyển
route trong web).

---

## 6. CRUD — ý tưởng cốt lõi

**Ý tưởng quan trọng nhất của cả app:** Quản lý bất kỳ bảng nào cũng chỉ gồm 4
việc giống hệt nhau — **C**reate (thêm), **R**ead (đọc), **U**pdate (sửa),
**D**elete (xoá). Gọi tắt là **CRUD**.

Vì 6 bảng đều làm 4 việc đó, mình viết phần chung **một lần duy nhất** trong
`CrudPage.cs`. Mỗi màn hình con chỉ cần "điền vào chỗ trống" phần riêng của nó:

| Phần cần điền | Ý nghĩa |
|---|---|
| `Title`, `IdColumn` | Tên hiển thị và tên cột khóa chính |
| `LoadData(search)` | Câu SELECT để đổ dữ liệu vào bảng (Read) |
| `BuildEditFields()` | Tạo các ô nhập ở panel bên phải |
| `FillFields(row)` | Khi bấm 1 dòng → đổ dữ liệu dòng đó vào các ô |
| `ClearFields()` | Xoá trống các ô (khi bấm "+ New") |
| `Save()` | Lưu: INSERT nếu mới, UPDATE nếu đang sửa (Create/Update) |
| `Delete()` | Xoá dòng đang chọn (Delete) |

> 👉 Đây là lý do code **không phức tạp**: học 1 màn hình là hiểu cả 6, vì chúng
> dùng chung khuôn mẫu.

Bố cục mỗi màn hình trông như thế này:
```
┌──────────┬───────────────────────────────────────────────────┐
│          │  Tutors           [ tìm kiếm ] [Refresh] [+ New]   │
│  Menu    ├─────────────────────────────────┬─────────────────┤
│          │                                 │  Details        │
│Dashboard │      Bảng dữ liệu (grid)        │  Name   [.....] │
│Tutors    │      (bấm vào 1 dòng để sửa)    │  Subject[.....] │
│Students  │                                 │  Rate   [.....] │
│...       │                                 │  [Save] [Delete]│
└──────────┴─────────────────────────────────┴─────────────────┘
```

---

## 7. Đọc hiểu TutorsPage.cs

Mở file `Pages/TutorsPage.cs` và đọc cùng mình. Đây là màn hình quản lý gia sư,
và nó có chú thích "STEP 1" đến "STEP 6" trong code.

**STEP 1 — READ (đọc dữ liệu để hiện lên bảng):**
```csharp
return Database.Query(
    "SELECT tutor_id, name, subject, hourly_rate, level FROM tutors ORDER BY tutor_id");
```
→ Lấy tất cả gia sư, sắp theo id, đổ vào bảng. Nếu người dùng gõ tìm kiếm thì
thêm `WHERE name ILIKE @q` để lọc.

**STEP 2 — Tạo các ô nhập ở panel Details:**
```csharp
_name    = AddTextField("Name");       // ô chữ
_subject = AddTextField("Subject");
_rate    = AddNumberField("Hourly rate", 0, 10_000_000);  // ô số
_level   = AddTextField("Level");
```
→ `AddTextField`, `AddNumberField` là các hàm có sẵn trong `CrudPage.cs` giúp tạo
ô nhập kèm nhãn, chỉ 1 dòng.

**STEP 3 — Khi bấm 1 dòng trong bảng, đổ dữ liệu vào các ô:**
```csharp
_name.Text = row["name"]?.ToString() ?? "";
```
→ `row["name"]` là lấy giá trị cột `name` của dòng đang chọn. `?? ""` nghĩa là
"nếu trống thì để chuỗi rỗng" (tránh lỗi).

**STEP 4 — Xoá trống ô (khi bấm "+ New"):**
```csharp
_name.Text = "";  _subject.Text = "";  ...
```

**STEP 5 — Lưu (quan trọng nhất):**
```csharp
if (Current == null)   // Current == null nghĩa là KHÔNG có dòng nào đang chọn → thêm mới
{
    Database.Execute("INSERT INTO tutors (...) VALUES (@name, @subject, @rate, @level)",
        Database.P("name", _name.Text), ... );
}
else                   // đang chọn 1 dòng → cập nhật dòng đó
{
    Database.Execute("UPDATE tutors SET name=@name, ... WHERE tutor_id=@id",
        Database.P("name", _name.Text), ..., Database.P("id", Current["tutor_id"]));
}
```

**Vì sao dùng `@name` và `Database.P("name", ...)` mà không ghép chuỗi thẳng?**
Đây là điểm cực kỳ quan trọng về **an toàn**. KHÔNG BAO GIỜ viết kiểu:
```csharp
// ❌ SAI - nguy hiểm!
"INSERT INTO tutors (name) VALUES ('" + _name.Text + "')"
```
Vì nếu người dùng gõ tên có dấu nháy `'` hoặc code độc, nó sẽ làm hỏng/tấn công
database (lỗi gọi là **SQL Injection** — chắc bạn đã nghe trong web). Cách đúng là
dùng **tham số**: viết `@name` trong câu SQL, rồi đưa giá trị thật qua
`Database.P("name", _name.Text)`. Thư viện sẽ tự xử lý an toàn.

**STEP 6 — Xoá:**
```csharp
Database.Execute("DELETE FROM tutors WHERE tutor_id=@id", Database.P("id", Current!["tutor_id"]));
```

Xong! 6 bước này lặp lại y hệt ở mọi màn hình khác. Khác biệt duy nhất ở
Classes/Enrollments/... là chúng có thêm **dropdown** cho khóa ngoại (chọn tên gia
sư/học viên thay vì gõ id).

---

## 8. Luồng chạy của app

Từ lúc bạn bấm chạy:

1. **`Program.cs`** chạy đầu tiên → gọi `Database.TestConnection()` để thử kết nối
   database. Nếu lỗi → hiện thông báo dễ hiểu rồi dừng. Nếu OK → mở `MainForm`.
2. **`MainForm`** vẽ thanh menu bên trái và mở sẵn màn hình **Dashboard**.
3. Bạn **bấm một mục menu** (ví dụ "Tutors") → `MainForm` tạo `TutorsPage` (lần đầu)
   và hiện nó ra vùng nội dung.
4. `TutorsPage` khi hiện ra → gọi `LoadData()` → `Database.Query()` chạy SELECT →
   PostgreSQL trả dữ liệu → hiện lên bảng.
5. Bạn bấm 1 dòng → dữ liệu đổ sang panel Details. Sửa → bấm **Save** → chạy
   UPDATE → bảng được tải lại.

Giống hệt vòng đời một trang web CRUD, chỉ là chạy trên desktop.

---

## 9. Code có phức tạp không?

**Không.** Và nó được thiết kế có chủ đích để đơn giản:

- **Chia lớp rõ ràng:** mỗi file một việc (giống kiến trúc web bạn quen: tách
  API, component, style).
- **Lặp lại một khuôn mẫu:** 6 màn hình giống nhau, học 1 hiểu 6.
- **Không có "ma thuật" ẩn:** mọi câu SQL đều viết rõ ràng, bạn đọc là hiểu nó
  làm gì.

Phần "khó" duy nhất là `CrudPage.cs` (lớp khuôn mẫu dùng kỹ thuật kế thừa —
`abstract`/`override`). Nhưng bạn **không cần sửa file đó**. Cứ coi nó như một
"thư viện nội bộ" đã làm sẵn, bạn chỉ dùng.

> Nếu thấy khó, hãy nhớ: bạn chỉ cần hiểu **1 file `TutorsPage.cs`** là đủ để tự
> thêm/sửa mọi thứ trong app.

---

## 10. Cách tự chỉnh sửa

### Đổi màu / giao diện
Mở `Theme.cs`, sửa giá trị màu. Ví dụ đổi màu nút chính → sửa dòng `Accent`.

### Thêm một ô nhập mới (ví dụ thêm "Email" cho Students)
1. Database phải có cột `email` trong bảng `students` trước.
2. Trong `StudentsPage.cs`:
   - Thêm biến: `private TextBox _email = null!;`
   - Trong `BuildEditFields()`: `_email = AddTextField("Email");`
   - Trong `FillFields()`: `_email.Text = row["email"]?.ToString() ?? "";`
   - Trong `ClearFields()`: `_email.Text = "";`
   - Trong `LoadData()`: thêm `email` vào câu SELECT.
   - Trong `Save()`: thêm `email` vào INSERT/UPDATE + `Database.P("email", _email.Text)`.

### Thêm hẳn một màn hình mới
1. Copy `TutorsPage.cs` → đổi tên file và tên class, sửa tên bảng/cột.
2. Trong `MainForm.cs`, thêm 1 dòng: `AddNav("Tên Mới", () => new TenMoiPage());`

### Đổi thông tin kết nối database
Sửa đúng 1 dòng trong `Database.cs` (chuỗi `ConnString`).

---

## 11. Bảng tra thuật ngữ

| Thuật ngữ | Nghĩa dễ hiểu |
|-----------|----------------|
| **WinForms** | Công nghệ làm app desktop Windows bằng C# |
| **Control** | Một thành phần giao diện (ô nhập, nút, bảng...) — như thẻ HTML |
| **Form / UserControl** | Một cửa sổ / một màn hình — như một trang web |
| **CRUD** | Create-Read-Update-Delete: 4 thao tác với dữ liệu |
| **SQL** | Ngôn ngữ để ra lệnh cho database |
| **Table (bảng)** | Nơi lưu dữ liệu, có cột và dòng — như sheet Excel |
| **Row / Column** | Dòng / cột trong bảng |
| **Primary Key (khóa chính)** | Cột định danh duy nhất mỗi dòng (vd `tutor_id`) |
| **Foreign Key (khóa ngoại)** | Cột trỏ sang bảng khác (vd `classes.tutor_id`) |
| **Parameter (tham số)** | `@name` — chỗ điền giá trị an toàn vào SQL |
| **SQL Injection** | Lỗ hổng bảo mật khi ghép chuỗi SQL ẩu — ta tránh bằng tham số |
| **PostgreSQL** | Phần mềm database đang dùng |
| **Npgsql** | Thư viện giúp C# nói chuyện với PostgreSQL |
| **DataGridView** | Control hiển thị bảng dữ liệu (cái bảng lớn bạn thấy) |
| **ComboBox** | Ô chọn dạng dropdown |
| **Inheritance (kế thừa)** | Một class "mượn" lại code của class khác (CrudPage) |

---

### Lời kết
App của bạn **đúng chuẩn** cho một công cụ quản trị. Trông "giống database
dashboard" là vì nó đúng là như vậy — và đó là điều tốt. Hãy bắt đầu bằng việc
đọc `TutorsPage.cs` cùng tài liệu này, rồi thử tự thêm một ô nhập. Khi quen rồi,
cả 6 màn hình sẽ trở nên rất dễ với bạn. Chúc bạn học vui! 🎉
