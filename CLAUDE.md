# Giasu123 — Admin App

A Windows desktop (WinForms) admin panel for the **Giasu123** tutoring website.
It lets an administrator view and manage everything in the database: tutors,
students, classes, enrollments, payments and reviews.

> This file is written for a beginner. It explains **what each part does** and
> **how to change things safely**. Read it top to bottom once and the project
> will make sense.

---

## 1. How to run it

**In Visual Studio:** open `Giasu123.slnx`, press **F5** (or the green ▶ "Start" button).

**From a terminal** (PowerShell), inside the repo folder:

```powershell
dotnet run --project Giasu123
```

When it starts it first checks it can reach the database. If it can't, you'll
see a clear error message instead of a crash.

---

## 2. What you need installed

| Thing | Why | Notes |
|-------|-----|-------|
| .NET 10 SDK | to build & run the app | `dotnet --version` should work |
| PostgreSQL | the database | must be **running** when you start the app |
| Npgsql | the driver that connects C# to PostgreSQL | already added via NuGet, nothing to do |

### Database connection
The app connects to a PostgreSQL database named **`legal_text2sql`**.
The login details live in **one place**: `Giasu123/Database.cs`:

```
Host=localhost;Port=5432;Database=legal_text2sql;Username=postgres;Password=123
```

If your PostgreSQL username/password/port is different, **edit that one line**.

---

## 3. The database tables

The app manages these 6 tables (they are linked by "foreign keys" — a column in
one table that points to a row in another):

```
tutors ──┐
         ├──< classes ──< enrollments ──< payments
students ┘        (a class belongs      (a payment is for
   │               to a tutor)           one enrollment)
   └──< reviews >── tutors
         (a review is written by a student about a tutor)
```

| Table | Columns | Meaning |
|-------|---------|---------|
| `tutors` | tutor_id, name, subject, hourly_rate, level | the teachers |
| `students` | student_id, name, grade, city | the learners |
| `classes` | class_id, subject, grade, fee, **tutor_id** | a course taught by a tutor |
| `enrollments` | enrollment_id, **student_id**, **class_id**, enroll_date, status | a student joining a class |
| `payments` | payment_id, **enrollment_id**, amount, payment_date, method, status | money paid for an enrollment |
| `reviews` | review_id, **tutor_id**, **student_id**, rating (1–5), comment, review_date | feedback on a tutor |

**Bold** columns are foreign keys — in the app these appear as drop-down lists so
you pick a name instead of typing an id number.

---

## 4. How the code is organised

Every file has one clear job. Here's the map:

```
Giasu123/
├─ Program.cs            → starts the app; checks the DB connection first
├─ Database.cs           → the ONLY file that talks to PostgreSQL
├─ Theme.cs              → all colors, fonts and the "good looking" styling
├─ MainForm.cs           → the main window: dark sidebar + content area
└─ Pages/
   ├─ CrudPage.cs        → the shared blueprint every table screen is built on
   ├─ DashboardPage.cs   → the overview screen (stat cards + recent payments)
   ├─ TutorsPage.cs      → manage tutors   ← the fully-commented EXAMPLE
   ├─ StudentsPage.cs    → manage students
   ├─ ClassesPage.cs     → manage classes      (has a Tutor drop-down)
   ├─ EnrollmentsPage.cs → manage enrollments  (Student + Class drop-downs)
   ├─ PaymentsPage.cs    → manage payments     (Enrollment drop-down)
   └─ ReviewsPage.cs     → manage reviews       (Tutor + Student drop-downs)
```

### The big idea: one pattern, reused everywhere
Managing any table is the same four jobs — **C**reate, **R**ead, **U**pdate,
**D**elete (CRUD). So the shared layout and logic live once in **`CrudPage.cs`**,
and each table's page only fills in the specifics:

- `Title` / `IdColumn` — the name shown and the primary-key column
- `LoadData(search)` — the `SELECT` that fills the table
- `BuildEditFields()` — which input boxes appear on the right
- `FillFields(row)` — copy a clicked row into those boxes
- `ClearFields()` — empty the boxes (for a new record)
- `Save()` — INSERT (new) or UPDATE (existing)
- `Delete()` — remove the selected row

👉 **Start by reading `Pages/TutorsPage.cs`.** It is commented step-by-step.
Once you understand it, every other page is the same shape.

### What a page looks like on screen
```
┌───────────┬──────────────────────────────────────────────────────┐
│           │  Tutors              [ search ] [Refresh] [+ New]      │
│  Sidebar  ├────────────────────────────────────┬─────────────────┤
│  (menu)   │                                     │  Details        │
│           │        the data table (grid)        │  Name  [......]  │
│ Dashboard │                                     │  Subject [....]  │
│ Tutors    │                                     │  Rate  [......]  │
│ Students  │                                     │  Level [......]  │
│ ...       │                                     │  [ Save ][Delete]│
└───────────┴────────────────────────────────────┴─────────────────┘
```

---

## 5. How to use the app

1. **Pick a table** from the left sidebar.
2. **Browse / search** — type in the search box to filter the table live.
3. **Edit a record** — click a row; its values appear in the *Details* panel on
   the right. Change them and click **Save**.
4. **Add a record** — click **+ New** (top right), fill the *Details* panel,
   click **Save**.
5. **Delete a record** — click a row, then **Delete** (you'll be asked to confirm).

The **Dashboard** shows totals (tutors, students, revenue, average rating) and
the 25 most recent payments.

---

## 6. How to make common changes

### Change the look (colors / fonts)
Edit **`Theme.cs`**. For example, to change the main button color, change
`Accent`. Everything updates at once because every control reads from `Theme`.

### Add a field to an existing table screen
Example — add an "email" box to Students:
1. In `StudentsPage.cs`, add `private TextBox _email = null!;`
2. In `BuildEditFields()`: `_email = AddTextField("Email");`
3. In `FillFields()`: `_email.Text = row["email"]?.ToString() ?? "";`
4. In `ClearFields()`: `_email.Text = "";`
5. In `LoadData()` add `email` to the `SELECT`, and in `Save()` add it to the
   INSERT/UPDATE (plus a `Database.P("email", _email.Text)` parameter).
   *(The `email` column must also exist in the database table.)*

### Add a whole new table screen
1. Create `Pages/MyThingPage.cs`, copy `TutorsPage.cs`, and adjust the table
   name, columns and fields.
2. In `MainForm.cs`, add one line in the sidebar section:
   `AddNav("My Thing", () => new MyThingPage());`
   (Remember: buttons are added bottom-to-top, so put it where you want it.)

---

## 7. Important rules the code follows (please keep these)

- **All database access goes through `Database.cs`.** Never write a connection
  string anywhere else.
- **Always use parameters** (`@name` + `Database.P("name", value)`), never glue
  user text directly into SQL. This prevents a security hole called *SQL
  injection* and handles quotes/empty values correctly.
- **The UI is built in C# code, not the drag-and-drop designer.** This is on
  purpose: it keeps each screen in one readable file that's easy to learn from.

---

## 8. Troubleshooting

| Problem | Likely cause / fix |
|---------|--------------------|
| "Database connection failed" on startup | PostgreSQL isn't running, or the details in `Database.cs` are wrong. |
| A delete fails with a foreign-key error | The row is still referenced elsewhere (e.g. a tutor that still has classes). Delete the dependent rows first. |
| A drop-down is empty | The related table has no rows yet (e.g. add a tutor before adding a class). |
| Changes don't appear | Click **Refresh**, or re-select the table in the sidebar. |

---

## 9. Tech summary (for reference)

- **Language/UI:** C# + WinForms, targeting `net10.0-windows`
- **Database:** PostgreSQL, accessed with the **Npgsql** driver
- **Architecture:** a thin data layer (`Database.cs`) + a reusable page base
  (`CrudPage.cs`) + one page per table + a sidebar shell (`MainForm.cs`)
