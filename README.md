# HTMLDownloader

**HTMLDownloader** is an open-source web scraping and archiving tool built with C# and .NET Framework, using Windows Forms and WebView2 to handle dynamic content and save pages as MHTML.

## Installation

### End-user (recommended)
<p align="center">
  <img src="docs/img/install.gif" alt="Install with MSI" width="800"><br>
  <em>Install with MSI</em>
</p>

<p align="center">
  <img src="docs/img/portable.gif" alt="Run portable ZIP" width="800"><br>
  <em>Run portable ZIP</em>
</p>

Download from the **Releases** page:
- `HTMLDownloader-v1.0.msi` — installer (recommended)
- `HTMLDownloader-v1.0-win32|win64.zip` — portable (no installation)

> Requirements: .NET Framework 4.8 and Microsoft Edge WebView2 Runtime (see `docs/RUNTIME.md`).

### Developer (build from source)
1. Clone this repository: `git clone https://github.com/truongbavinh/HTMLDownloader-.git`
2. Open in Visual Studio 2019/2022.
3. Restore NuGet packages (including WebView2).
4. Build and run the project.
## Download samples
Four sample files are bundled so you can run both flows immediately:
- `samples/urls.category.farfetch.txt`  
- `samples/urls.products.farfetch.txt`  
- `samples/urls.category.rightmove.txt`  
- `samples/urls.products.rightmove.txt`  
## Documentation
- Quick Start (End-User): [docs/USER_GUIDE.md](docs/USER_GUIDE.md)
- Runtime Requirements: [docs/RUNTIME.md](docs/RUNTIME.md)
- Security & Trust: [docs/SECURITY.md](docs/SECURITY.md)
- **Target Framework Rationale**: [docs/FRAMEWORK.md](docs/FRAMEWORK.md)
- Reproducibility: [docs/REPRODUCIBILITY.md](docs/REPRODUCIBILITY.md)
- **Reproducible capsule (DOI):** https://doi.org/10.5281/zenodo.16935169
## Usage
- Enter URLs in the input field.
- Select a save directory.
- Click "Download" to start.

## License
MIT License (see LICENSE.txt).

## Contributions
Fork this repository, submit pull requests, or report issues to improve the tool.

## Metadata Table

| Field                     | Description                                                                                          |
|---------------------------|------------------------------------------------------------------------------------------------------|
| Title                     | HTMLDownloader: An Open-Source Tool for Dynamic Web Scraping and Archiving Using WebView2           |
| Authors                   | Ba-Vinh Truong, Loan T.T. Nguyen, Phu Pham, Bay Vo                                                   |
| Affiliations              | 1. Faculty of Information Technology, HUTECH University, Ho Chi Minh City, Vietnam  <br> 2. School of Computer Science and Engineering, International University, Ho Chi Minh City, Vietnam  <br> 3. Vietnam National University, Ho Chi Minh City, Vietnam |
| Contact Emails            | tbvinh25nct@hutech.edu.vn, nttloan@hcmiu.edu.vn, pta.phu@hutech.edu.vn, vd.bay@hutech.edu.vn        |
| Repository URL            | https://github.com/truongbavinh/HTMLDownloader-                                                      |
| License                   | MIT                                                                                                  |
| Version                   | 1.0                                                                                                  |
| Year                      | 2025                                                                                                 |
| Programming Language      | C#                                                                                                   |
| Toolkits/Technologies     | Microsoft Edge WebView2, .NET Framework                                                              |
| Operating System          | Windows 10+ (WebView2 Runtime required)                                                              |
| How to Run                | Open the `.sln` solution in Visual Studio and build. Then run the application.                       |
| Input                     | List of URLs (e.g., from a textbox or file import inside the app)                                    |
| Output                    | HTML content archived locally on disk                                                                |
| Documentation             | Included in `README.md`                                                                              |
| Reproducibility           | Fully reproducible with provided C# source code and clear setup instructions                         |
