using System;

[Flags]
public enum CloudRegionFlag
{
	eu = 1,
	us = 2,
	asia = 4,
	jp = 8,
	au = 0x10,
	usw = 0x20,
	sa = 0x40,
	cae = 0x80,
	kr = 0x100,
	@in = 0x200,
	ru = 0x400,
	rue = 0x800
}
