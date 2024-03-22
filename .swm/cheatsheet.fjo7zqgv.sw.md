---
title: Cheatsheet
---
<SwmSnippet path="/BBplus/Content/test.bbp" line="5">

---

This code snippet defines a function `myFunction` that sends a `message` set to 'Hello!'.

```bbp
myFunction => fn() {
    message: "Hello!";
}
```

---

</SwmSnippet>

<SwmSnippet path="/BBplus/Content/test.bbp" line="9">

---

This code snippet defines a module `Player` with functions to move the player horizontally and vertically. The `Player` module has two variables `x` and `y` representing the player's current position.

```bbp
mod Player {
    // Initialise the player's position
    // Parameters: x - the x position
    //             y - the y position
    x => 0; y => 0;
    
    // Function to move the player horizontally
    // Parameters: units - the number of units/pixels to move
    movePlayerX => @fn(units) {
        message: "Moving player by", units, "units horizontally.";
        x => x + units;
    }
    
    // Function to move the player vertically
    // Parameters: units - the number of units/pixels to move
    movePlayerY => @fn(units) {
        message: "Moving player by", units, "units vertically.";
        y => y + units;
    }
}
```

---

</SwmSnippet>

<SwmMeta version="3.0.0" repo-id="Z2l0aHViJTNBJTNBQkJQbHVzJTNBJTNBQnJhaW5Cb3gtSW50ZXJhY3RpdmU=" repo-name="BBPlus"><sup>Powered by [Swimm](https://app.swimm.io/)</sup></SwmMeta>
