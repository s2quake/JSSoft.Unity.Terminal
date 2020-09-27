# 소개

유니티에서 터미널 형태의 개발 환경을 구현한 라이브러리입니다.

# 개발 환경

    Unity 2018.4.27f1

# 만들기

### 터미널
    
    GameObject -> UI -> Terminals -> Terminal

### 터미널 + 명령어

    GameObject -> UI -> Terminals -> Terminal - Commands

### 터미널 레이아웃

    GameObject -> UI -> Terminals -> TerminalLayout

## 터미널 레이아웃 + 터미널

    GameObject -> UI -> Terminals -> Terminal Full

### 터미널 레이아웃 + 터미널 + 명령어

    GameObject -> UI -> Terminals -> Terminal Full - Commands

# 터미널 보이기 / 감추기

    GameObject -> UI -> Terminals -> Terminal Full - Commands 메뉴에서 터미널 생성

    Hierarchy 창에서 Canvas/TerminalRoot 객체 선택

    Inspector 창에서 Terminal Rect Visible Controller 컴포넌트에서 키 값과 방향 값을 설정

    기본 키값은 Control + ` 이며 숨겨지는 방향은 위쪽

    Play 이후 Control + ` 키로 터미널 보이기 또는 감추기

# 터미널에 로그 표시하기

    GameObject -> UI -> Terminals -> Terminal Full - Commands 메뉴에서 터미널 생성

    Hierarchy 창에서 Canvas/TerminalRoot/TerminalLayout/Terminal 객체 선택

    Inspector 창에서 Add Component 클릭후 Terminal Log Receiver 추가
    
# 명령어 추가하기

### 명령어 만들기
```csharp
using JSSoft.Unity.Terminal;
using JSSoft.Unity.Terminal.Commands;
using UnityEngine.SceneManagement;

public class RestartCommand : TerminalCommandBase
{
    public RestartCommand(ITerminal terminal)
        : base(terminal)
    {
    }

    protected override void OnExecute()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
```

### 명령어 제공자 만들기
```csharp
using System.Collections.Generic;
using JSSoft.Library.Commands;
using JSSoft.Unity.Terminal;
using JSSoft.Unity.Terminal.Commands;
using UnityEngine;

[RequireComponent(typeof(CommandContextHost))]
public class CommandSystem : MonoBehaviour, ICommandProvider
{
    private readonly CommandProvider commands = new CommandProvider();

    private void Awake()
    {
        GetComponent<CommandContextHost>().CommandProvider = this;
    }

    #region ICommandProvider

    IEnumerable<ICommand> ICommandProvider.Provide(ITerminal terminal)
    {
        foreach (var item in commands.Provide(terminal))
        {
            yield return item;
        }
        yield return new RestartCommand(terminal);
    }

    #endregion
}
```

### 컴포넌트 추가하기

    GameObject -> UI -> Terminals -> Terminal Full - Commands 메뉴에서 터미널 생성

    Hierarchy 창에서 Canvas/TerminalRoot/TerminalLayout/Terminal 객체 선택

    Inspector 창에서 Add Component 클릭후 Command System 추가

    Play 후 프롬프트창에서 restart 명령어 실행

# 터미널 속성 만들기

### 속성 만들기
```csharp
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JSSoft.Unity.Terminal.Commands;

public class TestConfiguration : MonoBehaviour
{
    [SerializeField]
    private float value = 0;
    [SerializeField]
    private ConfigurationSystem configSystem = null;

    private void Awake()
    {
        if (this.configSystem != null)
        {
            this.configSystem.AddConfig(new FieldConfiguration("test.value", this, nameof(value)) { DefaultValue = this.value });
        }
    }
}
```

### 속성 제공자 만들기
```csharp
using System.Collections.Generic;
using JSSoft.Library.Commands;
using JSSoft.Unity.Terminal;
using JSSoft.Unity.Terminal.Commands;
using UnityEngine;

[RequireComponent(typeof(CommandContextHost))]
public class ConfigurationSystem : MonoBehaviour, ICommandConfigurationProvider
{
    private readonly CommandConfigurationProvider configs = new CommandConfigurationProvider();

    private void Awake()
    {
        GetComponent<CommandContextHost>().ConfigurationProvider = this;
    }

    public void AddConfig(ICommandConfiguration provider)
    {
        this.configs.Add(provider);
    }

    #region ICommandConfigurationProvider

    IEnumerable<ICommandConfiguration> ICommandConfigurationProvider.Configs => configs.Configs;

    #endregion
}
```

### 속성 제공자 컴포넌트 추가하기

    GameObject -> UI -> Terminals -> Terminal Full - Commands 메뉴에서 터미널 생성

    Hierarchy 창에서 Canvas/TerminalRoot/TerminalLayout/Terminal 객체 선택

    Inspector 창에서 Add Component 클릭후 Configuration System 추가


### 속성 객체 만들기

    GameObject -> Create Empty로 객체 생성

    Hierarchy 창에서 새로 생성된 GameObject 객체 선택

    Inspector 창에서 Add Component 클릭후 Test Configuration 추가

    Test Configuration 컴포넌트의 Config System 의 값을 Terminal로 선택

### 런타임상에서 속성 사용하기

    Play후 프롬프트에서 config --list 실행하여 config의 값을 확인

    프롬프트에서 config test.value 1 실행하여 값을 1로 변경

    프롬프트에서 config test.value --reset 실행하여 기본값으로 설정
