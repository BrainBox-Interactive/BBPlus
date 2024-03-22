---
title: Modules
---
<SwmSnippet path="/BBplus/Content/test.bbp" line="9">

---

This code snippet defines a `Player` module with functions to initialize and move the player's position. The `movePlayerX` function moves the player horizontally by a specified number of units, while the `movePlayerY` function moves the player vertically by a specified number of units.

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

<SwmSnippet path="/BBplus/Content/test.bbp" line="42">

---

This code snippet moves the player in the X-axis by 1 unit.

```bbp
Player.movePlayerX: 1;
```

---

</SwmSnippet>

<SwmSnippet path="/BBplus/Content/test.bbp" line="46">

---

This code snippet moves the player in the Y-axis by 1 unit.

```bbp
Player.movePlayerY: 1;
```

---

</SwmSnippet>

<SwmMeta version="3.0.0" repo-id="Z2l0aHViJTNBJTNBQkJQbHVzJTNBJTNBQnJhaW5Cb3gtSW50ZXJhY3RpdmU=" repo-name="BBPlus"><sup>Powered by [Swimm](https://app.swimm.io/)</sup></SwmMeta>
