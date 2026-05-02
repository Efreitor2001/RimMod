# Ремонт через ПКМ (RimWorld 1.6)

## Что делает мод
Добавляет пункты в контекстное меню (ПКМ):
- Починить оружие
- Починить одежду/броню
- Починить снаряжение пешки при клике по пешке

## Требования
- RimWorld 1.6
- Harmony

## Сборка
1. Откройте `Source/RepairViaContextMenu/RepairViaContextMenu.csproj`.
2. Укажите переменную `RimWorldPath` (путь к папке RimWorld).
3. Соберите проект в Release.
4. Скопируйте `RepairViaContextMenu.dll` в `Assemblies/`.

## Локализация
Весь пользовательский текст находится в `Languages/Russian/Keyed/RepairViaContextMenu.xml`.


## Примечание по сборке
Если `RimWorldPath` не задан или `0Harmony.dll` не найден, проект подтянет Harmony из NuGet (`Lib.Harmony`) только для компиляции исходников. Для запуска в игре всё равно используется Harmony из RimWorld/модов.
