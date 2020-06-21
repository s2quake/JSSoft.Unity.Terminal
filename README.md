# Terminal

## 개요

유니티에서 터미널과 비슷한 환경을 사용하도록 구현한 라이브러리입니다.

흔히 콘솔창으로 많이 알려져 있는 이 기능은 개발 과정에 도움이 될 수 있도록

명령어 기반의 개발 환경을 제공합니다.

![base](./terminal.gif)
![4](./terminal-4.png)
![16](./terminal-16.png)

## 개발 환경

    Unity 2018.4.20f1
    UGUI

## 폴더 구조

### Assets/Plugins/JSSoft.Terminal

    터미널 라이브러리

### Assets/Plugins/JSSoft.Communication.Services

    터미널을 테스트 하기 위한 통신 모듈

## 컴포넌트 구조

### Terminal.cs

    시각적 의존성 없이 터미널의 논리적인 부분만을 구현한 것입니다.
    명령어 실행, 명령어 조합, 명령어내의 커서 위치, 문자열 출력 등이 이에 해당됩니다.

### TerminalGrid.cs

    논리적으로 구현된 터미널을 시각적으로 표시하도록 해주는 객체입니다.
    이 객체는 터미널의 정보를 기반으로 Row와 Cell로 표현되며 사용자와의 상호 동작을 터미널에 전달합니다.
    대부분의 컴포넌트는 Terminal보다 TerminalGrid와 연관성이 높습니다.

### TerminalBackground

    터미널에서 표현되는 글자의 배경색을 나타냅니다.

### TerminalForeground

    터미널에서 표현되는 글자의 표현과 전경색을 나타냅니다.
    이 컴포넌트가 직접적으로 표시하지 않으며 하위 객체인 TerminalForegroundItem을 관리합니다.

#### TerminalForegroundItem

    실제적으로 터미널의 글자를 표현하는 컴포넌트입니다. TerminalForeground에서 관리되고 있으며 글자 텍스쳐당 한개의 TerminalForegroundItem 으로 나타냅니다.

### TerminalCursor

    터미널의 명령 프롬프트의 커서를 나타냅니다.

### TerminalComposition

    터미널의 명령 프롬프트에서 IME(input method editor)에 의해 만들어지는 글자를 표현하는 컴포넌트입니다.
    이 컴포넌트가 직접적으로 표시하지는 않으며 하위 객체를 관리하는 관리자 역할을 합니다.

#### TerminalCompositionBackground

    IME에 의해 만들어지는 글자의 배경색을 나타냅니다.

#### TerminalCompositionForeground

    IME에 의해 만들어지는 글자의 표현과 전경색을 나타냅니다.

### TerminalScrollbar

    터미널의 스크롤바를 나타냅니다.
