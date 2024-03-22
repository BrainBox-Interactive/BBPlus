---
title: Functions
---
&nbsp;

<SwmSnippet path="/BBplus/Content/test.bbp" line="5">

---

This code snippet defines a function `myFunction` that returns an object with a `message` property set to 'Hello!'.

```bbp
myFunction => fn() {
    message: "Hello!";
}
```

---

</SwmSnippet>

<SwmSnippet path="/BBplus/Content/test.bbp" line="24">

---

This code snippet defines a function `movePlayerY` that takes a parameter `units`. It returns an object with a `message` property describing how many units the player is being moved vertically, and a `y` property representing the new vertical position of the player.

```bbp
    movePlayerY => @fn(units) {
        message: "Moving player by", units, "units vertically.";
        y => y + units;
    }
```

---

</SwmSnippet>

<SwmSnippet path="/BBplus/Content/test.bbp" line="30">

---

This code snippet defines a function `waitSeconds` that takes a parameter `seconds`. It changes the console color to yellow, prints a message indicating the number of seconds to wait, waits for the specified number of seconds, and then changes the console color back to white.

```bbp
waitSeconds => @fn(seconds) {
    consoleColor: Yellow;
    
    if (seconds == 1) { message: "Waiting for", seconds, "second."; }
    else { message: "Waiting for", seconds, "seconds."; }
    
    wait: seconds * 1000;
    consoleColor: White;
}
```

---

</SwmSnippet>

&nbsp;

<SwmMeta version="3.0.0" repo-id="Z2l0aHViJTNBJTNBQkJQbHVzJTNBJTNBQnJhaW5Cb3gtSW50ZXJhY3RpdmU=" repo-name="BBPlus"><sup>Powered by [Swimm](https://app.swimm.io/)</sup></SwmMeta>
